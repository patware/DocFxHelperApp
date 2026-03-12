using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Engine
{
  public class NaiveSequentialProcessor(ILogger<NaiveSequentialProcessor> logger, Drops.IScout scout, Sources.IIngestion ingestion) : IProcessor
  {
    private readonly ILogger<NaiveSequentialProcessor> _logger = logger;
    private readonly Drops.IScout _scout = scout;
    private readonly Sources.IIngestion _ingestion = ingestion;

    public async Task ProcessAsync(Domain.Run run)
    {

      _logger.LogInformation("Run {RunId} trigger by {TriggerSource} {TriggerAuthor} in working directory {WorkingDirectory}",
        run.Id, run.Trigger.Source, run.Trigger.Author, run.WorkingDirectory);

      var drop = Abstractions.Engine.BuildPaths.Resolve(run.WorkingDirectory).Drop;

      var readySources = _scout.Recon(drop);

      foreach (var source in readySources.OrderBy(s => s.LastWriteTimeUtc))
      {
        await _ingestion.IngestAsync(source);
      }

      _logger.LogInformation("Completed processing run {RunId}", run.Id);
    }
  }
}
