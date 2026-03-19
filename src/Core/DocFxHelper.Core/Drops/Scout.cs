using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Drops
{
  public class Scout(
    ILogger<Scout> logger,
    DocFxHelper.Infrastructure.IFileSystem fileSystem) : IScout
  {
    private readonly ILogger<Scout> _logger = logger;
    private readonly DocFxHelper.Infrastructure.IFileSystem _fileSystem = fileSystem;

    public IReadOnlyList<System.IO.DirectoryInfo> Recon(string drop)
    {
      _logger.LogInformation("Recon {drop}", drop);

      var readySources = new List<System.IO.DirectoryInfo>();

      foreach (var subFolder in _fileSystem.GetDirectories(drop, false))
      {
        var sourceSpecJson = System.IO.Path.Combine(subFolder, "source.spec.json");
        var masterSpecJson = System.IO.Path.Combine(subFolder, "master.spec.json");
        var buildSpecJson = System.IO.Path.Combine(subFolder, "build.spec.json");

        if (File.Exists(sourceSpecJson))
        {
          if (File.Exists(buildSpecJson))
          {
            _logger.LogInformation(" [{folder}] is ready, found both the [source.spec.json] and [build.spec.json]", System.IO.Path.GetRelativePath(drop, subFolder));
            readySources.Add(new DirectoryInfo(subFolder));
          }
          else
          {
            _logger.LogInformation(" [{folder}] is not ready, found a [source.spec.json] but not the [build.spec.json]", System.IO.Path.GetRelativePath(drop, subFolder));
          }
        }
        else if(File.Exists(masterSpecJson))
        {
          if (File.Exists(buildSpecJson))
          {
            _logger.LogInformation(" [{folder}] is ready, found both the [master.spec.json] and [build.spec.json]", System.IO.Path.GetRelativePath(drop, subFolder));
            readySources.Add(new DirectoryInfo(subFolder));
          }
          else
          {
            _logger.LogInformation(" [{folder}] is not ready, found a [master.spec.json] but not the [build.spec.json]", System.IO.Path.GetRelativePath(drop, subFolder));
          }
        }
        else
        {
          _logger.LogInformation("Didn't find any master.spec.json or source.spec.json in {folder}", subFolder);
        }
      }

      _logger.LogInformation("Recon returning [{count}] folders ready to be ingested", readySources.Count);

      return readySources;
    }
  }
}
