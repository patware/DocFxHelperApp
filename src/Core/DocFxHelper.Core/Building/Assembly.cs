using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using DocFxHelper.Core.Utils;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.RepresentationModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DocFxHelper.Core.Building
{
  public class Assembly(
    ILogger<Assembly> logger,
    IFileSystem fileSystem,
    ITocHelper tocHelper,
    IEnumerable<IDocFxBuildSourceContributor> sourceContributors
    ) : IAssembly
  {

    private readonly ILogger<Assembly> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly ITocHelper _tocHelper = tocHelper;
    private readonly IReadOnlyList<IDocFxBuildSourceContributor> _sourceContributors = sourceContributors.ToList();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
      WriteIndented = true
    };


    public async Task Assemble(BuildPaths buildPaths, Graph.SiteGraph siteGraph, CancellationToken ct = default!)
    {
      _logger.LogInformation("Assembly started");

      _logger.LogInformation("Clean Staging");

      var folders = _fileSystem.GetDirectories(buildPaths.Staging, false);
      foreach (var folder in folders)
      {
        try
        {
          _fileSystem.DeleteDirectory(folder);
        }
        catch (Exception ex)
        {
          _logger.LogWarning("Couldn't delete folder {folder} - skipping.  Exception: {message}", folder, ex.Message);
        }
      }

      var files = _fileSystem.GetFiles(buildPaths.Staging, "*", false);

      foreach (var file in files)
      {
        try
        {
          _fileSystem.DeleteFile(file);
        }
        catch(Exception ex)
        {
          _logger.LogWarning("Couldn't delete file {file} - skipping.  Exception: {message}", file, ex.Message);
        }
      }

      _fileSystem.CreateDirectory(buildPaths.Staging);

      _logger.LogInformation("Copy Sources to Staging");
      foreach (var kvp in siteGraph.Sources)
      {
        CopySourceToStaging(kvp.Value, buildPaths);
      }

      _logger.LogInformation("Process Mustache Templates");
      ProcessMustacheTemplates(buildPaths, siteGraph);

      _logger.LogInformation("Link child sources to their parent");
      await LinkChildSourcesToParent(buildPaths, siteGraph, ct);

      _logger.LogInformation("Generate DocFx Config");
      var docFx = await GenerateDocFxConfigAsync(buildPaths, siteGraph, ct);

      await SaveDocFxJsonAsync(buildPaths, docFx, ct);

      _logger.LogInformation("Save DocFx Config to docfx.json");

      _logger.LogInformation("Assembly completed");


    }
    

    private async Task LinkChildSourcesToParent(BuildPaths buildPaths, SiteGraph siteGraph, CancellationToken ct = default!)
    {
      var queue = new System.Collections.Generic.Queue<SiteNode>();

      queue.Enqueue(siteGraph.Root);

      while (queue.Count > 0)
      {
        var siteNode = queue.Dequeue();

        foreach (var child in siteNode.Children)
        {
          queue.Enqueue(child);

          await LinkChildSourceToParent(buildPaths, siteNode, child, ct);
        }

      }
    }

    private async Task LinkChildSourceToParent(BuildPaths buildPaths, SiteNode parent, SiteNode child, CancellationToken ct)
    {
      if (!child.ShowInToc)
      {
        return;
      }

      var parentNodePath = System.IO.Path.Combine(buildPaths.Staging, parent.Id);      
      var parentSubFolder = System.IO.Path.Combine(parentNodePath, child.Path);
      var parentPathLevels = new DirectoryInfo(parentNodePath).FullName.Split(System.IO.Path.DirectorySeparatorChar).Length;
      var parentTocPath = _fileSystem.FindFileUpwards(parentSubFolder, "toc.yml", parentPathLevels);

      if (parentTocPath == null)
      {
        if (_fileSystem.DirectoryExists(parentSubFolder))
        {
          _fileSystem.CreateDirectory(parentSubFolder);
        }
        parentTocPath = System.IO.Path.Combine(parentSubFolder, "toc.yml");
      }

      var parentTocFolder = System.IO.Path.GetDirectoryName(parentTocPath)!;

      string yaml;

      if (_fileSystem.FileExists(parentTocPath))
      {
        yaml = await _fileSystem.ReadAllTextAsync(parentTocPath, ct);
      }
      else
      {
        yaml = "items:";
      }

      var toc = _tocHelper.GetToc(yaml);

      List<TocItem> items;

      if (string.IsNullOrEmpty(child.ParentTocDisplayName))
      {
        items = toc.Items;
      }
      else
      {
        var parentTocItem = toc.Items.FirstOrDefault(ti => string.Equals(ti.Name, child.ParentTocDisplayName, StringComparison.InvariantCultureIgnoreCase));

        parentTocItem ??= new TocItem
          {
            Name = child.ParentTocDisplayName
          };

        parentTocItem.Items ??= new List<TocItem>();

        items = parentTocItem.Items!;
      }

      var childTocItem = items.FirstOrDefault(ti => string.Equals(ti.Name, child.DisplayName, StringComparison.InvariantCultureIgnoreCase));

      if (childTocItem == null)
      {
        childTocItem ??= new TocItem
        {
          Name = child.DisplayName
        };

        if (child.TocItemInsertAtIndex == null)
        {
          items.Add(childTocItem);
        }
        else
        {
          items.Insert(child.TocItemInsertAtIndex.Value, childTocItem);
        }
      }

      var childNodePath = System.IO.Path.Combine(buildPaths.Staging, child.Id);
      var childPathRelativeToParentFolder = System.IO.Path.GetRelativePath(parentTocFolder, childNodePath);

      childTocItem.Href = string.Concat(childPathRelativeToParentFolder, Path.DirectorySeparatorChar);

      yaml = _tocHelper.GetString(toc);

      await _fileSystem.WriteAllTextAsync(parentTocPath, yaml, ct);

    }

    private async Task SaveDocFxJsonAsync(BuildPaths buildPaths, JsonNode docFx, CancellationToken ct = default!)
    {
      var docfxJson = System.IO.Path.Combine(buildPaths.Staging, "docfx.json");

      var json = docFx.ToJsonString(_jsonSerializerOptions);

      await _fileSystem.WriteAllTextAsync(docfxJson, json, ct);

    }

    private async Task<JsonNode> GenerateDocFxConfigAsync(BuildPaths buildPaths, SiteGraph siteGraph, CancellationToken ct = default!)
    {

      await using var stream = await OpenBaseDocFxStreamAsync(buildPaths, siteGraph);

      var nodeOptions = new System.Text.Json.Nodes.JsonNodeOptions
      {
        PropertyNameCaseInsensitive = true
      };

      var documentOptions = new System.Text.Json.JsonDocumentOptions();

      var baseDocFx = await System.Text.Json.Nodes.JsonObject.ParseAsync(stream, nodeOptions, documentOptions, ct);

      var root = baseDocFx?.AsObject() ?? throw new InvalidOperationException("docfx.json root must be a JSON object.");

      var build = GetOrCreateObjectStrict(root, "build");

      ApplySourceFragments(build, siteGraph);
      ApplyBuildDefaults(build);

      return root;
    }

    private void ApplySourceFragments(JsonObject build, SiteGraph siteGraph)
    {
      var content = GetOrCreateArrayStrict(build, "content");
      var resource = GetOrCreateArrayStrict(build, "resource");
      var globalMetadata = GetOrCreateObjectStrict(build, "globalMetadata");
      var fileMetadata = GetOrCreateObjectStrict(build, "fileMetadata");

      foreach (var node in siteGraph.Sources.Values)
      {
        var contributor = ResolveContributor(node.SourceSpec);
        var fragment = contributor.Create(node);

        AppendEntries(content, fragment.Content);
        AppendEntries(resource, fragment.Resource);
        MergeObject(globalMetadata, fragment.GlobalMetadata);
        MergeObject(fileMetadata, fragment.FileMetadata);
        MergeTemplates(build, fragment.Templates);
      }
    }

    private IDocFxBuildSourceContributor ResolveContributor(SourceSpec sourceSpec)
    {
      var contributor = _sourceContributors.FirstOrDefault(x => x.CanHandle(sourceSpec));

      return contributor ?? throw new InvalidOperationException($"No DocFX build contributor registered for source type '{sourceSpec.GetType().Name}'.");
    }

    private static void ApplyBuildDefaults(JsonObject build)
    {
      build["output"] = "_site";

      if (!build.TryGetPropertyValue("template", out var templateNode) || templateNode is null)
      {
        build["template"] = new JsonArray("default", "modern");
      }
    }

    private static void AppendEntries(JsonArray target, IEnumerable<JsonObject> entries)
    {
      foreach(var entry in entries)
      {
        target.Add(entry.DeepClone());
      }
    }

    private static void MergeTemplates(JsonObject build, IEnumerable<string> templates)
    {
      var templateArray = GetOrCreateArrayStrict(build, "template");

      var existing = new HashSet<string>(
        templateArray
          .Select(x => x?.GetValue<string>())
          .Where(x => !string.IsNullOrWhiteSpace(x))!,
        StringComparer.OrdinalIgnoreCase
      );

      foreach (var template in templates) 
      {
        if (existing.Add(template))
        {
          templateArray.Add(template);
        }
      }

    }

    private static void MergeObject(JsonObject target, JsonObject source)
    {
      foreach(var kvp in source)
      {
        if (kvp.Value is null)
        {
          continue;
        }

        if (kvp.Value is JsonObject sourceObj && target.TryGetPropertyValue(kvp.Key, out var existingNode) && existingNode is JsonObject targetObj)
        {
          MergeObject(targetObj, sourceObj);
          continue;
        }

        target[kvp.Key] = kvp.Value.DeepClone();
      }

    }

    private static Task<Stream> OpenBaseDocFxStreamAsync(BuildPaths buildPaths,SiteGraph siteGraph)
    {
      if (siteGraph.BuildContext.Master.DocFxJsonPath != null)
      {
        var docFxJson = System.IO.Path.Combine(buildPaths.Sources, "_master", siteGraph.BuildContext.Master.DocFxJsonPath);
        Stream stream = new FileStream(docFxJson, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
      }
      else
      {
        Stream empty = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
        return Task.FromResult(empty);
      }
    }


    private static JsonArray GetOrCreateArrayStrict(JsonObject parent, string propertyName)
    {
      if (!parent.TryGetPropertyValue(propertyName, out JsonNode? child) || child is null)
      {
        var created = new JsonArray();
        parent[propertyName] = created;
        return created;
      }

      if (child is JsonArray array)
      {
        return array;
      }

      throw new InvalidOperationException(
        $"Expected '{propertyName}' to be a JSON array.");

    }

    //private static JsonObject CreateSourceContent(SiteNode source)
    //{
    //  var dest = string.Empty;

    //  if (!string.IsNullOrEmpty(source.Path))
    //  {
    //    dest = source.Path;
    //  }

    //  return new JsonObject
    //  {
    //    ["files"] = new JsonArray("**/*.{md,yml}"),
    //    ["exclude"] = new JsonArray("_site/**"),
    //    ["src"] = string.Concat(source.Id, "/"),
    //    ["dest"] = dest
    //  };

    //}

    private static JsonObject GetOrCreateObjectStrict(JsonObject parent, string propertyName)
    {
      if (!parent.TryGetPropertyValue(propertyName, out JsonNode? child) || child is null)
      {
        var created = new JsonObject();
        parent[propertyName] = created;
        return created;
      }

      if (child is JsonObject obj)
      {
        return obj;
      }

      var kind = child switch
      {
        JsonArray => "array",
        JsonValue => "value",
        _ => "node"
      };

      throw new InvalidOperationException(
        $"Expected '{propertyName}' to be a JSON object, but found a JSON {kind}.");
    }

    private void ProcessMustacheTemplates(BuildPaths buildPaths, SiteGraph siteGraph)
    {
      foreach (var templateMeta in siteGraph.BuildContext.Master.Templates)
      {
        var mustacheTemplatePath = System.IO.Path.Combine(buildPaths.Sources, templateMeta.TemplateFile);
        var outFile = System.IO.Path.Combine(buildPaths.Staging, templateMeta.OutputFile);

        _logger.LogInformation("Template [{template}] to [{dest}]", mustacheTemplatePath, outFile);
      }
    }

    private void CopySourceToStaging(SiteNode siteNode, BuildPaths buildPaths)
    {
      string fromFolder;

      var sourcePathInConverted = buildPaths.SourceInConverted(siteNode.Id);

      if (_fileSystem.DirectoryExists(sourcePathInConverted))
      {
        fromFolder = sourcePathInConverted;
      }
      else
      {
        fromFolder = buildPaths.SourceInSources(siteNode.Id);
      }

      var target = buildPaths.SourceInStaging(siteNode.Id);

      _fileSystem.CopyDirectory(fromFolder, target);
    }
  }
}
