using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Utils;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DocFxHelper.Core.Building
{
  public class Assembly(
    ILogger<Assembly> logger,
    IFileSystem fileSystem
    ) : IAssembly
  {

    private readonly ILogger<Assembly> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions
    {
      WriteIndented = true
    };


    public async Task Assemble(BuildPaths buildPaths, Graph.SiteGraph siteGraph, CancellationToken ct = default!)
    {
      _logger.LogInformation("Assembly started");

      _logger.LogInformation("Copy Sources to Staging");
      foreach(var kvp in siteGraph.Sources)
      {
        CopySourceToStaging(kvp.Value, buildPaths);
      }
            
      _logger.LogInformation("Process Mustache Templates");
      ProcessMustacheTemplates(buildPaths, siteGraph);

      _logger.LogInformation("Generate DocFx Config");
      var docFx = await GenerateDocFxConfigAsync(buildPaths, siteGraph);

      await SaveDocFxJsonAsync(buildPaths, docFx, ct);

      _logger.LogInformation("Save DocFx Config to docfx.json");

      _logger.LogInformation("Assembly completed");
      return;

    }

    private async Task SaveDocFxJsonAsync(BuildPaths buildPaths, JsonNode docFx, CancellationToken ct = default!)
    {
      var docfxJson = System.IO.Path.Combine(buildPaths.Staging, "docfx.json");

      var json = docFx.ToJsonString(_jsonSerializerOptions);

      await _fileSystem.WriteAllTextAsync(docfxJson, json, ct);

    }

    private async Task<JsonNode> GenerateDocFxConfigAsync(BuildPaths buildPaths, SiteGraph siteGraph, CancellationToken ct = default!)
    {
      Stream stream;

      if (siteGraph.BuildContext.Master.DocFxJsonPath != null)
      {
        var docFxJson = System.IO.Path.Combine(buildPaths.Sources, "_master", siteGraph.BuildContext.Master.DocFxJsonPath);
        stream = new FileStream(docFxJson, FileMode.Open, FileAccess.Read);
      }
      else
      {        
        stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));         
      }

      var nodeOptions = new System.Text.Json.Nodes.JsonNodeOptions { 
        PropertyNameCaseInsensitive = true
      };

      var documentOptions = new System.Text.Json.JsonDocumentOptions
      {
      };

      var baseDocFx = await System.Text.Json.Nodes.JsonObject.ParseAsync(stream, nodeOptions, documentOptions, ct);

      var root = baseDocFx?.AsObject() ?? throw new InvalidOperationException("docfx.json root must be a JSON object.");

      var build = GetOrCreateObjectStrict(root, "build");

      var content = GetOrCreateArrayStrict(build, "content");

      foreach (var source in siteGraph.Sources)
      {
        content.Add(CreateSourceContent(source.Value));
      }

      build["output"] = "_site";

      if (!build.TryGetPropertyValue("template", out JsonNode? template) || template is null)
      {
        build["template"] = new JsonArray("default", "modern");
      }
      
      return baseDocFx;
    }
    

    private static JsonArray GetOrCreateArrayStrict(JsonObject parent,string propertyName)
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

    private static JsonObject CreateSourceContent(SiteNode source)
    {
      var dest = string.Empty;

      if (!string.IsNullOrEmpty(source.Path))
      {
        dest = source.Path;
      }

      return new JsonObject
      {
        ["files"] = new JsonArray("**/*.{md,yml}"),
        ["exclude"] = new JsonArray("_site/**"),
        ["src"] = string.Concat(source.Id, "/"),
        ["dest"] = dest
      };

    }

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
      foreach(var templateMeta in siteGraph.BuildContext.Master.Templates)
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
