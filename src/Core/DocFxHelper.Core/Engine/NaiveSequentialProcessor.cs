using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Utils;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Engine
{
  public class NaiveSequentialProcessor(
    ILogger<NaiveSequentialProcessor> logger, 
    Utils.IGeneral general,
    Drops.IScout scout,
    Sources.IIngestion ingestion,
    IVerification verification,
    Graph.IGraphBuilder graphBuilder,
    Convert.IConversion conversionService,
    Building.IAssembly assembly) : IProcessor
  {
    private readonly ILogger<NaiveSequentialProcessor> _logger = logger;
    private readonly Utils.IGeneral _general = general;
    private readonly Drops.IScout _scout = scout;
    private readonly Sources.IIngestion _ingestion = ingestion;
    private readonly IVerification _verification = verification;
    private readonly IGraphBuilder _graphBuilder = graphBuilder;
    private readonly Convert.IConversion _conversionService = conversionService;
    private readonly Building.IAssembly _assembly = assembly;


    public async Task ProcessAsync(Domain.Run run, CancellationToken ct = default!)
    {

      _logger.LogInformation("Run {RunId} trigger by {TriggerSource} {TriggerAuthor} in working directory {WorkingDirectory}",
        run.Id, run.Trigger.Source, run.Trigger.Author, run.WorkingDirectory);

      _logger.LogInformation("Step 0 - Prep work");
      var buildPaths = Abstractions.Engine.BuildPaths.Resolve(run.WorkingDirectory);

      _general.EnsureExists(buildPaths);

      _logger.LogInformation("Step 1 - Recon");
      var readyDrops = _scout.Recon(buildPaths.Drop);

      if (readyDrops.Any())
      {
        _logger.LogInformation("Step 2 - Ingestion");
        foreach (var item in readyDrops.OrderBy(s => s.LastWriteTimeUtc))
        {
          await _ingestion.IngestAsync(item.FullName, buildPaths.Sources, ct);
        }
        _logger.LogInformation("Step 2 - Ingestion: Done");
      }
      else
      {
        _logger.LogInformation("Step 2 - Ingestion: skipped, nothing to ingest");
      }


      _logger.LogInformation("Step 3 - Conversion");

      var siteGraph = await _graphBuilder.BuildAsync(buildPaths.Sources, ct);

      if (siteGraph.BuildContext == null)
      {
        _logger.LogInformation("Step 3 - Build context is null, exiting");
        return;
      }
  
      if (siteGraph.BuildContext.Sources.Any())
      {
        foreach (var source in siteGraph.BuildContext.Sources.Values)
        {
          await _conversionService.ConvertAsync(buildPaths, source, ct);
        }

        _logger.LogInformation("Step 3 - Conversion: Done");

      }
      else
      {
        _logger.LogInformation("Step 3 - Conversion: skipped, no sources in _sources folder");
      }

      _logger.LogInformation("Step 4 - PreAssembly verification");
      _logger.LogInformation("Step 4 - PreAssembly verification - Orphan folders");
      var orphans = _verification.GetOrphanFolders(buildPaths, siteGraph.BuildContext);

      if (orphans.Any())
      {
        _logger.LogInformation("Number of orphan folders: {count}", orphans.Count);
        foreach (var orphan in orphans)
        {
          _general.MoveToOrphanFolder(orphan, buildPaths);
        }
      }
      else
      {
        _logger.LogInformation("No orphan folder");
      }

      _logger.LogInformation("Step 5 - Assembly");
      await _assembly.Assemble(buildPaths, siteGraph, ct);

      _logger.LogInformation("Step 6 - Compilation (not implemented yet)");

      _logger.LogInformation("Step 7 - Publication (not implemented yet)");

      _logger.LogInformation("Completed processing run {RunId}", run.Id);
    }
  }
}
