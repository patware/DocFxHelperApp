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
    IAssemblyHelper assemblyHelper
    ) : IAssembly
  {

    private readonly ILogger<Assembly> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;
    
    private readonly IAssemblyHelper _assemblyHelper = assemblyHelper;



    public async Task Assemble(BuildPaths buildPaths, Graph.SiteGraph siteGraph, CancellationToken ct = default!)
    {
      _logger.LogInformation("Assembly started");

      _logger.LogInformation("Clean Staging");

      _assemblyHelper.CleanStaging(buildPaths);

      _fileSystem.CreateDirectory(buildPaths.Staging);

      _logger.LogInformation("Copy Sources to Staging");
      foreach (var kvp in siteGraph.Items)
      {
        _assemblyHelper.CopySourceToStaging(kvp.Value.Item, buildPaths);
      }

      _logger.LogInformation("Process Mustache Templates");
      _assemblyHelper.ProcessMustacheTemplates(buildPaths, siteGraph);

      _logger.LogInformation("Link child sources to their parent");
      await _assemblyHelper.LinkChildSourcesToParent(buildPaths, siteGraph, ct);

      _logger.LogInformation("Generate DocFx Config");
      var docFx = await _assemblyHelper.GenerateDocFxConfigAsync(buildPaths, siteGraph, ct);

      await _assemblyHelper.SaveDocFxJsonAsync(buildPaths, docFx, ct);

      _logger.LogInformation("Save DocFx Config to docfx.json");

      _logger.LogInformation("Assembly completed");


    }


  }
}
