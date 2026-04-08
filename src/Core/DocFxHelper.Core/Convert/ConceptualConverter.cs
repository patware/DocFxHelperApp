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
  public class ConceptualConverter(
    ILogger<ConceptualConverter> logger,
    Infrastructure.IFileSystem fileSystem) : BaseConverter<Specs.ConceptualSourceSpec>
  {
    private readonly ILogger<ConceptualConverter> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;

    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
      .UseYamlFrontMatter()
      .Build();

    public override async Task<int> Convert(Abstractions.Engine.BuildPaths buildPaths, ConceptualSourceSpec sourceSpec, BuildSpec buildSpec, CancellationToken ct = default)
    {
      _logger.LogInformation("---------------");
      _logger.LogInformation("Conceptual [{id}] Conversion started", sourceSpec.Id);

      _logger.LogInformation("Task 0 - Move files from Sources/ to Converted/");

      var sourceFolder = System.IO.Path.Combine(buildPaths.Sources, sourceSpec.Id);
      var convertedFolder = System.IO.Path.Combine(buildPaths.Converted, sourceSpec.Id);

      if (_fileSystem.DirectoryExists(convertedFolder))
      {
        _logger.LogInformation("Deleting source folder [{converted}] - clean folder", Path.GetRelativePath(buildPaths.WorkingDirectory, convertedFolder));
        _fileSystem.DeleteDirectory(convertedFolder);
      }

      _logger.LogInformation("Copying to [{converted}]", Path.GetRelativePath(buildPaths.WorkingDirectory, convertedFolder));
      _fileSystem.CopyDirectory(sourceFolder, convertedFolder);

      var siteBase = new Uri(sourceSpec.CloneUrl, UriKind.Absolute);
      _logger.LogInformation("Conceptual base is {siteBase}", siteBase);

      _logger.LogInformation("Fetch list of .md files");
      var mdFiles = _fileSystem.GetFiles(convertedFolder, "*.md", true);

      _logger.LogInformation("Number of .md files: {countOfMdFiles}", mdFiles.Count);

      _logger.LogInformation("Task 1 - Set Initial Yaml Headers");

      foreach (var mdFile in mdFiles)
      {
        await SetInitialYamlHeaders(convertedFolder, siteBase, sourceSpec.DocsFolder, mdFile, ct);
      }

      _logger.LogInformation("Conceptual [{id}] Converted", sourceSpec.Id);

      return 0;
    }


    private string GetMdUid(string rootPath, Uri siteBase, string mdFilePath)
    {
      var relativePath = Path.GetRelativePath(rootPath, mdFilePath);

      return (new Uri(siteBase, relativePath.Replace("\\", "/"))).ToString();
    }

    private async Task SetInitialYamlHeaders(string rootPath, Uri siteBase, string docsFolder, string mdFilePath, CancellationToken ct = default)
    {
      var markdown = await _fileSystem.ReadAllTextAsync(mdFilePath, ct);

      var mdUrl = GetMdUid(rootPath, siteBase, mdFilePath);
      var docUrl = GetDocUrl(rootPath, siteBase, docsFolder,  mdFilePath);

      var document = Markdown.Parse(markdown, _pipeline);
      var yamlHeader = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

      if (yamlHeader == null)
      {
        var yaml =
        $"""
        ---
        uid: {mdUrl}
        docurl: {docUrl}
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

    private static string GetDocUrl(string rootPath, Uri siteBase, string docsFolder, string mdFilePath)
    {
      var mdFileFolderPath = Path.GetDirectoryName(mdFilePath); 
      var mdFileBase = Path.GetFileName(mdFilePath);
      var mdRelativePath = Path.GetRelativePath(rootPath, string.Concat(mdFileFolderPath, "/", mdFileBase));
      var localhost = new Uri(string.Concat("http://localhost/", mdRelativePath));

      var docUrl = string.Concat(siteBase.AbsoluteUri, "?path=", docsFolder,  localhost.AbsolutePath);

      return docUrl;
    }

  }
}
