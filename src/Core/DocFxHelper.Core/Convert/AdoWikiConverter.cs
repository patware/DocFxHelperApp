using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Specs;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace DocFxHelper.Core.Convert
{
  public class AdoWikiConverter(
    ILogger<AdoWikiConverter> logger,
    Infrastructure.IFileSystem fileSystem,
    Utils.ITocHelper tocHelper) : BaseConverter<Specs.AdoWikiSourceSpec>
  {
    private readonly ILogger<AdoWikiConverter> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;
    private readonly Utils.ITocHelper _tocHelper = tocHelper;

    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
      .UseYamlFrontMatter()
      .Build();

    private readonly IDeserializer _deserializer = new DeserializerBuilder().Build();

    private readonly Uri _baseUri = new("https://localhost/");

    public override async Task Convert(Abstractions.Engine.BuildPaths buildPaths, AdoWikiSourceSpec sourceSpec, CancellationToken ct = default)
    {
      _logger.LogInformation("Ado Wiki Conversion started");

      _logger.LogInformation("Step 0 - Move Wiki from Sources/ to Converted/");

      var sourceFolder = System.IO.Path.Combine(buildPaths.Sources, sourceSpec.Id);
      var convertedFolder = System.IO.Path.Combine(buildPaths.Converted, sourceSpec.Id);

      if (_fileSystem.DirectoryExists(convertedFolder))
      {
        _logger.LogInformation("Deleting source folder [{converted}] - clean folder", Path.GetRelativePath(buildPaths.WorkingDirectory, convertedFolder));
        _fileSystem.DeleteDirectory(convertedFolder);
      }

      _logger.LogInformation("Copying to [{converted}]", Path.GetRelativePath(buildPaths.WorkingDirectory, convertedFolder));
      _fileSystem.CopyDirectory(sourceFolder, convertedFolder);

      var wikiBase = new Uri(EnsureTrailingSlash(sourceSpec.WikiUrl), UriKind.Absolute);
      _logger.LogInformation("Wiki base is {wikiBase}", wikiBase);

      _logger.LogInformation("Fetch list of .md files");
      var mdFiles = _fileSystem.GetFiles(convertedFolder, "*.md", true);

      _logger.LogInformation("Number of .md files: {countOfMdFiles}", mdFiles.Count);

      _logger.LogInformation("Step 1 - Set Initial Yaml Headers");
      foreach (var mdFile in mdFiles)
      {
        await SetInitialYamlHeaders(convertedFolder, wikiBase, mdFile, ct);
      }

      _logger.LogInformation("Step 2 - Snapshot .order files to the corresponding md file guid");
      var dotOrderFiles = _fileSystem.GetFiles(convertedFolder, ".order", true);
      foreach(var dotOrderFile in dotOrderFiles)
      {
        await Snapshot(convertedFolder, wikiBase, dotOrderFile, ct);
      }

      var promotionDic = new Dictionary<string, string>();

      if (sourceSpec.Promote != null && sourceSpec.Promote.Any())
      {
        _logger.LogInformation("Step 3 - Promote Pages - {count} specified", sourceSpec.Promote.Count);

        foreach (var promote in sourceSpec.Promote)
        {
          promotionDic.Add(promote, Promote(convertedFolder, promote));
        }
      }
      else
      {
        _logger.LogInformation("Step 3 - Promote Pages - none specified");
      }

      if (promotionDic.Any())
      {
        mdFiles = _fileSystem.GetFiles(convertedFolder, "*.md", true);
      }

      _logger.LogInformation("Step 3 - Prepare Hyperlinks");
      foreach (var mdFile in mdFiles)
      {
        await PrepareHyperlinks(convertedFolder, wikiBase, mdFile, ct);
      }

      _logger.LogInformation("Step 4 - Rename [md Files] to DocFx safe name format");
      foreach (var mdFile in mdFiles)
      {
        await RenameMdFilesToDocFxSafeFormat(convertedFolder, wikiBase, mdFile, ct);
      }

      _logger.LogInformation("Step 5 - Rename [Folders] to DocFx safe name format");
      var renamedFoldersDic = await RenameFoldersToDocFxSafeFormat(convertedFolder, ct);

      _logger.LogInformation("Step 6 - Finalize Hyperlinks");

      _logger.LogInformation("Step 7 - Update Mermaid Code Delimiters");
      mdFiles = _fileSystem.GetFiles(convertedFolder, "*.md", true);
      foreach(var mdFile in mdFiles)
      {
        await UpdateMermaidCodeDelimiters(mdFile, ct);
      }

      _logger.LogInformation("Step 8 - Set each page's UID");

      _logger.LogInformation("Step 9 - Convert .order to toc.yml");

      var folders = renamedFoldersDic.Values;

      foreach(var folder in folders)
      {
        if (!System.IO.Path.GetFileName(folder).StartsWith('.'))
        {
          await EnsureDotOrderExistsAsync(folder, ct);

        }
      }

      var rootDi = new DirectoryInfo(convertedFolder);
      var dotOrders = _fileSystem.GetFiles(convertedFolder, ".order", true);

      foreach (var dotOrder in dotOrders)
      {
        var isTopNav = sourceSpec.TopMenuReferenced 
          && string.Equals(rootDi.FullName, new DirectoryInfo(Path.GetDirectoryName(dotOrder)!).FullName, StringComparison.InvariantCultureIgnoreCase);

        await ConvertOrderToToc(dotOrder, isTopNav, promotionDic);
      }

      await Task.CompletedTask;

      _logger.LogInformation("{id} Converted", sourceSpec.Id);
    }

    private async Task UpdateMermaidCodeDelimiters(string mdFile, CancellationToken ct = default)
    {
      var markdown = await _fileSystem.ReadAllTextAsync(mdFile, ct);
      var updatedMarkdown = ConvertMermaidCodeDelimiters(markdown);

      if (!string.Equals(markdown, updatedMarkdown, StringComparison.Ordinal))
      {
        await _fileSystem.WriteAllTextAsync(mdFile, updatedMarkdown, ct);
      }
    }

    internal static string ConvertMermaidCodeDelimiters(string markdown)
    {
      var newline = markdown.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
      var lines = markdown.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
      var output = new List<string>(lines.Length);
      var insideMermaidBlock = false;

      foreach (var line in lines)
      {
        var trimmed = line.Trim();

        if (!insideMermaidBlock && string.Equals(trimmed, "::: mermaid", StringComparison.OrdinalIgnoreCase))
        {
          output.Add(line.Replace(trimmed, "``` mermaid", StringComparison.Ordinal));
          insideMermaidBlock = true;
          continue;
        }

        if (insideMermaidBlock && string.Equals(trimmed, ":::", StringComparison.Ordinal))
        {
          output.Add(line.Replace(trimmed, "```", StringComparison.Ordinal));
          insideMermaidBlock = false;
          continue;
        }

        output.Add(line);
      }

      return string.Join(newline, output);
    }

    private async Task<IReadOnlyDictionary<string,string>> RenameFoldersToDocFxSafeFormat(string folder, CancellationToken ct)
    {
      var ret = new Dictionary<string,string>();

      var childFolders = _fileSystem.GetDirectories(folder, false);

      foreach(var childFolder in childFolders)
      {
        var renames = await RenameFoldersToDocFxSafeFormat(childFolder, ct);

        foreach (var rename in renames)
        {
          ret.Add(rename.Key, rename.Value);
        }
      }

      ret.Add(folder, folder);

      var folderName = new DirectoryInfo(folder).Name;

      var folderNameDecoded = System.Web.HttpUtility.UrlDecode(folderName);

      if (folderNameDecoded != folderName)
      {
        string safeFolderName = GetSafeFilename(folderName);
        _logger.LogInformation("Folder [{folderName}] needs to be renamed to DocFx file name safe format [{}]", folderName, safeFolderName);

        ret[folder] = _fileSystem.RenameDirectory(folder, safeFolderName);
      }


      return ret;
    }

    private string Promote(string path, string promote)
    {
      var dic = new Dictionary<string, string>();

      var subFolderName = Path.GetFileNameWithoutExtension(promote);
      var subFolderPath = Path.Combine(path, subFolderName);

      if (!_fileSystem.DirectoryExists(subFolderPath))
      {
        _fileSystem.CreateDirectory(subFolderPath);
      }

      var destination = string.Empty;

      var destinationDefaultMd = Path.Combine(subFolderPath, $"{Utils.General.Default}.md");

      if (!_fileSystem.FileExists(destinationDefaultMd))
      {
        destination = destinationDefaultMd;
      }
      else
      {
        var destinationPromoteMd = Path.Combine(subFolderPath, subFolderName);

        if (!_fileSystem.FileExists(destinationPromoteMd))
        {
          destination = destinationPromoteMd;
        }
        else
        {
          int i = 0;

          destination = string.Empty;

          do
          {
            i++;

            var filenameWithIndex = Path.Combine(subFolderPath, string.Concat(subFolderPath, "_", i, ".md"));

            if (!_fileSystem.FileExists(filenameWithIndex))
            {
              destination = filenameWithIndex;
            }

          } while (string.IsNullOrEmpty(destination) && i < 100);
        }
      }

      if (string.IsNullOrEmpty(destination))
      {
        _logger.LogWarning("Couldn't find a filename to promote {promote} to.", promote);
        return Path.Combine(subFolderPath, promote);
      }

      var source = Path.Combine(path, promote);

      _fileSystem.MoveFile(source, destination);

      return destination;
    }

    private async Task EnsureDotOrderExistsAsync(string folder, CancellationToken ct = default!)
    {
      var dotOrder = System.IO.Path.Combine(folder, ".order");

      if (!(_fileSystem.FileExists(dotOrder)))
      {
        
        var mdFiles = _fileSystem.GetFiles(folder, "*.md", false);

        if (mdFiles.Any())
        {
          _logger.LogInformation("{dotOrder} does not exist, creating one from mdFiles list {count}", dotOrder, mdFiles.Count);
          var sb = new StringBuilder();

          foreach (var mdFile in mdFiles)
          {
            sb.AppendLine(System.IO.Path.GetFileNameWithoutExtension(mdFile));
          }

          await _fileSystem.WriteAllTextAsync(dotOrder, sb.ToString(), ct);
        }
      }

    }

    private async Task ConvertOrderToToc(string dotOrder, bool isTopNav, IDictionary<string, string> promotionDic)
    {
      string directory = Path.GetDirectoryName(dotOrder)!;

      var lines = await _fileSystem.ReadAllLinesAsync(dotOrder);

      var toc = new Graph.Toc();

      foreach(var line in lines)
      {
        var safeLine = GetSafeFilename(line);

        var mdFile = string.Concat(safeLine, ".md");

        if (string.Equals(safeLine, Utils.General.Default, StringComparison.InvariantCultureIgnoreCase))
        {
          if (lines.Length == 1)
          {
            toc.Items.Add(new Graph.TocItem
            {
              Name = safeLine,
              Href = mdFile
            });

          }
          continue;
        }
               

        var tocItem = new Graph.TocItem
        {
          Name = safeLine
        };

        toc.Items.Add(tocItem);

        

        if (promotionDic.ContainsKey(mdFile))
        {
          var promoted = promotionDic[mdFile];
          
          tocItem.Href = $"{safeLine}\\";
          tocItem.Homepage = $"{Path.GetRelativePath(directory, promoted)}";          
        }
        else
        {

          var mdFilePath = Path.Combine(directory, mdFile);
          var mdFolder = Path.Combine(directory, safeLine);

          var mdFileExists = _fileSystem.FileExists(mdFilePath);
          var mdFolderExists = _fileSystem.DirectoryExists(mdFolder);

          if (mdFileExists && mdFolderExists)
          {
            if (isTopNav)
            {
              tocItem.Href = $"{safeLine}/";
            }
            else
            {
              tocItem.Href = $"{safeLine}/toc.yml";
            }
            tocItem.Homepage = $"{safeLine}.md";
          }
          else if (mdFileExists)
          {
            tocItem.Href = $"{safeLine}.md";
          }
          else if (mdFolderExists)
          {
            if (isTopNav)
            {
              tocItem.Href = $"{safeLine}/";
            }
            else
            {
              tocItem.Href = $"{safeLine}/toc.yml";
            }
          }
          else
          {
            _logger.LogWarning("Edge case, neither {line}.md or folder name exists -> renamed ?", safeLine);
          }
        }        
      }

      var yaml = _tocHelper.GetString(toc);

      var tocYml = Path.Combine(directory!, "toc.yml");

      _logger.LogInformation("{dotOrder} => toc.yml", dotOrder);
      await _fileSystem.WriteAllTextAsync(tocYml!, yaml);
    }

    private async Task RenameMdFilesToDocFxSafeFormat(string convertedFolder, Uri wikiBase, string mdFile, CancellationToken ct)
    {
      var mdFilename = Path.GetFileName(mdFile);

      string safeFilename = GetSafeFilename(mdFilename);

      if (safeFilename != mdFilename)
      {
        _logger.LogInformation("md file [{mdFilename}] needs to be renamed to DocFx file name safe format [{}]", mdFilename, safeFilename);

        _fileSystem.RenameFile(mdFile, safeFilename);
      }
    }

    private static string GetSafeFilename(string name)
    {
      var mdFilenameDecoded = System.Web.HttpUtility.UrlDecode(name);

      if (string.Equals(mdFilenameDecoded, name, StringComparison.InvariantCultureIgnoreCase))
      {
        return name;
      }

      var safeFilenameReplaceKnownEscapes = name.Replace("\\(", "(").Replace("\\)", ")").Replace("-", " ");
      var safeFilename = System.Web.HttpUtility.UrlDecode(safeFilenameReplaceKnownEscapes);
      return safeFilename;
    }

    private async Task PrepareHyperlinks(string convertedFolder, Uri wikiBase, string mdFilePath, CancellationToken ct)
    {
      var markdown = await _fileSystem.ReadAllTextAsync(mdFilePath, ct);

      markdown = AdoWikiMarkdownFixer.FixAdoWikiEscapes(markdown);

      await _fileSystem.WriteAllTextAsync(mdFilePath, markdown);
    }

    private static string EnsureTrailingSlash(string url)
    {
      return url.EndsWith("/", StringComparison.Ordinal) ? url : string.Concat(url, "/");
    }

    private string GetMdUid(string rootPath, Uri wikiBase, string mdFilePath)
    {
      var relativePath = Path.GetRelativePath(rootPath, mdFilePath);

      return (new Uri(wikiBase, relativePath.Replace("\\", "/"))).ToString();
    }

    private async Task SetInitialYamlHeaders(string rootPath, Uri wikiBase, string mdFilePath, CancellationToken ct = default)
    {
      var markdown = await _fileSystem.ReadAllTextAsync(mdFilePath, ct);

      var mdUrl = GetMdUid(rootPath, wikiBase, mdFilePath);

      var document = Markdown.Parse(markdown, _pipeline);
      var yamlHeader = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

      if (yamlHeader == null)
      {
        var yaml =
        $"""
        ---
        uid: {mdUrl}
        ---
        """;

        markdown = yaml + "\n" + markdown;
      }
      else
      {
        var yaml = string.Join("\n",yamlHeader.Lines.Lines.Select(l => l.ToString()));
        
        var yamlStream = new YamlStream();
        yamlStream.Load(new StringReader(yaml));

        var uidKey = new YamlScalarNode("uid");

        var root = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        if (!root.Children.ContainsKey(uidKey))
        {
          root.Add(uidKey, new YamlScalarNode(mdUrl.ToString()));
        }

        var writer = new StringWriter();
        yamlStream.Save(writer, false);

        var newYaml =
        $"""
        ---
        {writer}
        ---
        """;

        markdown = markdown.Replace(yamlHeader.ToString()!, newYaml);

      }

      await _fileSystem.WriteAllTextAsync(mdFilePath, markdown, ct);

    }

    private async Task Snapshot(string convertedFolder, Uri wikiBase, string dotOrderFile, CancellationToken ct)
    {
      var snapshot = string.Concat(dotOrderFile, ".snapshot");

      var dotOrderFolder = System.IO.Path.GetDirectoryName(dotOrderFile)!;

      var lines = await _fileSystem.ReadAllLinesAsync(dotOrderFile, ct);

      await using var streamWriter = new StreamWriter(snapshot);
            
      foreach(var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
      {
        var mdFile = Path.Combine(dotOrderFolder, string.Concat(line, ".md"));
        var mdUrl = GetMdUid(convertedFolder, wikiBase, mdFile);

        await streamWriter.WriteLineAsync(mdUrl);
      }

    }

  }
}
