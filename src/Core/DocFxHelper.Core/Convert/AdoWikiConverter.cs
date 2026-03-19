using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Specs;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace DocFxHelper.Core.Convert
{
  public class AdoWikiConverter(
    ILogger<AdoWikiConverter> logger,
    Infrastructure.IFileSystem fileSystem) : BaseConverter<Specs.AdoWikiSourceSpec>
  {
    private readonly ILogger<AdoWikiConverter> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;
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

      _logger.LogInformation("Step 1 - Set Initial Yaml Headers");

      var wikiBase = new Uri(EnsureTrailingSlash(sourceSpec.WikiUrl), UriKind.Absolute);
      _logger.LogInformation("Wiki base is {wikiBase}", wikiBase);

      var mdFiles = _fileSystem.GetFiles(convertedFolder, "*.md", true);

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

      _logger.LogInformation("Step 6 - Move Root [md Files] that should actually be in their subfolder");

      _logger.LogInformation("Step 7 - Finalize Hyperlinks");

      _logger.LogInformation("Step 8 - Update Mermaid Code Delimiters");

      _logger.LogInformation("Step 9 - Set each page's UID");

      _logger.LogInformation("Step 10 - Convert .order to toc.yml");

      var folders = _fileSystem.GetDirectories(convertedFolder, true);
      foreach(var folder in folders)
      {
        if (!System.IO.Path.GetFileName(folder).StartsWith('.'))
        {
          await EnsureDotOrderExistsAsync(folder, ct);

        }
      }

      var dotOrders = _fileSystem.GetFiles(convertedFolder, ".order", true);

      foreach(var dotOrder in dotOrders)
      {
        await ConvertOrderToToc(dotOrder);
      }

      await Task.CompletedTask;

      _logger.LogInformation("{id} Converted", sourceSpec.Id);
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

    private async Task ConvertOrderToToc(string dotOrder)
    {
      string directory = Path.GetDirectoryName(dotOrder)!;

      var lines = await _fileSystem.ReadAllLinesAsync(dotOrder);

      var sb = new StringBuilder();

      sb.AppendLine("items:");

      foreach(var line in lines)
      {
        if (line != "Index")
        {
          var lineFileMd = System.IO.Path.Combine(directory, string.Concat(line, ".md"));
          var lineFolder = System.IO.Path.Combine(directory, line);

          var lineFileMdExists = _fileSystem.FileExists(lineFileMd);
          var lineFolderExists = _fileSystem.FolderExists(lineFolder);


          sb.AppendLine($"- name: {line}");

          if (lineFileMdExists && lineFolderExists)
          {
            sb.AppendLine($"  href: {line}/toc.yml");
            sb.AppendLine($"  homepage: {line}.md");
          }
          else if(lineFileMdExists)
          {
            sb.AppendLine($"  href: {line}.md");
          }
          else if (lineFolderExists)
          {
            sb.AppendLine($"  href: {line}/toc.yml");
          }
          else
          {
            _logger.LogWarning("Edge case, neither {line} .md or folder name exists -> renamed ?", line);
          }
          
        }
      }
           
      var tocYml = Path.Combine(directory!, "toc.yml");

      _logger.LogInformation("{dotOrder} => toc.yml", dotOrder);
      await _fileSystem.WriteAllTextAsync(tocYml!, sb.ToString());
    }

    private async Task RenameMdFilesToDocFxSafeFormat(string convertedFolder, Uri wikiBase, string mdFile, CancellationToken ct)
    {
      var mdFilename = Path.GetFileName(mdFile);

      var mdFilenameDecoded = System.Web.HttpUtility.UrlDecode(mdFilename);

      if (mdFilenameDecoded != mdFilename)
      {
        string safeFilename = GetSafeFilename(mdFilename);
        _logger.LogInformation("md file [{mdFilename}] needs to be renamed to DocFx file name safe format [{}]", mdFilename, safeFilename);

        _fileSystem.RenameFile(mdFile, safeFilename);
      }
    }

    private static string GetSafeFilename(string mdFilename)
    {
      var safeFilenameReplaceKnownEscapes = mdFilename.Replace("\\(", "(").Replace("\\)", ")").Replace("-", " ");
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
