using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Sources
{
  public class Ingestion(ILogger<Ingestion> logger) : IIngestion
  {
    private readonly ILogger<Ingestion> _logger = logger;

    public async Task IngestAsync(DirectoryInfo drop)
    {
      _logger.LogInformation("Ingesting drop {DropName} at path {DropPath}", drop.Name, drop.FullName);

      await Task.CompletedTask;

      _logger.LogInformation("Completed ingesting drop {DropName}", drop.Name);
    }
  }
}
