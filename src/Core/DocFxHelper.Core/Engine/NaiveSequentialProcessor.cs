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
    Convert.IConversion conversionService) : IProcessor
  {
    private readonly ILogger<NaiveSequentialProcessor> _logger = logger;
    private readonly Utils.IGeneral _general = general;
    private readonly Drops.IScout _scout = scout;
    private readonly Sources.IIngestion _ingestion = ingestion;
    private readonly Convert.IConversion _conversionService = conversionService;


    public async Task ProcessAsync(Domain.Run run)
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
          await _ingestion.IngestAsync(item.FullName, buildPaths.Sources);
        }
        _logger.LogInformation("Step 2 - Ingestion: Done");
      }
      else
      {
        _logger.LogInformation("Step 2 - Ingestion: skipped, nothing to ingest");
      }

      var sourceSpecs = await _ingestion.GetSpecsAsync(buildPaths.Sources);

      if (sourceSpecs.Any())
      {
        _logger.LogInformation("Step 3 - Conversion");

        foreach (var source in sourceSpecs)
        {
          await _conversionService.ConvertAsync(buildPaths, source);
        }

        _logger.LogInformation("Step 3 - Conversion: Done");

      }
      else
      {
        _logger.LogInformation("Step 3 - Conversion: skipped, no sources in _sources folder");
      }

      _logger.LogInformation("Step 4 - Assembly (not implemented yet)");

      _logger.LogInformation("Step 5 - Compilation (not implemented yet)");

      _logger.LogInformation("Step 6 - Publication (not implemented yet)");

      _logger.LogInformation("Completed processing run {RunId}", run.Id);
    }
  }
}
