using DocFxHelper.Core.Specs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocFxHelper.Core.Sources
{
  public class Ingestion(
    ILogger<Ingestion> logger,
    Infrastructure.IFileSystem fileSystem,
    ISpecLoader specLoader) : IIngestion
  {
    private readonly ILogger<Ingestion> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;
    private readonly ISpecLoader _specLoader = specLoader;

    public async Task IngestAsync(string itemPath, string sourcesPath)
    {
      _logger.LogInformation("Ingesting dropped {itemPath}", itemPath);

      var masterSpecJson = System.IO.Path.Combine(itemPath, Specs.MasterSpec.FileName);

      string? destinationFolder = null;

      if (_fileSystem.FileExists(masterSpecJson))
      {
        _logger.LogInformation("subFolder contains [master.spec.json]");
        destinationFolder = System.IO.Path.Combine(sourcesPath, "_master");
      }
      else
      {
        var sourceSpecJson = System.IO.Path.Combine(itemPath, Specs.SourceSpec.FileName);

        if (_fileSystem.FileExists(sourceSpecJson)) 
        {
          _logger.LogInformation("subfolder contains [source.spec.json], loading it to get the id");

          var sourceSpec = await _specLoader.LoadSourceSpecAsync(sourceSpecJson);

          if (sourceSpec != null)
          {
            _logger.LogInformation("The id is {id}, it will be used for the destination folder name", sourceSpec.Id);

            destinationFolder = System.IO.Path.Combine(sourcesPath, sourceSpec.Id);
          }
          else
          {
            throw new InvalidOperationException("The schema for the source.spec.json seems invalid, deserialization failed");
          }
        }        
      }

      if (destinationFolder == null)
      {
        _logger.LogWarning("subfolder didn't contain a valid [master.spec.json] or [source.spec.json], ignoring");
        return;
      }

      if (_fileSystem.DirectoryExists(destinationFolder))
      {
        _logger.LogInformation("{destinationFolder} exists, deleting to refresh it", destinationFolder);
        _fileSystem.DeleteDirectory(destinationFolder);
      }

      _fileSystem.MoveDirectory(itemPath, destinationFolder);

      _logger.LogInformation("{itemPath} was moved to {destinationFolder}", itemPath, destinationFolder);
      
      await Task.CompletedTask;

    }

    //public async Task<IReadOnlyList<Specs.SourceSpec>> GetSpecsAsync(string sourcesPath, CancellationToken ct = default)
    //{
    //  var sourceSpecs = new List<Specs.SourceSpec>();

    //  var subFolders = _fileSystem.GetDirectories(sourcesPath);

    //  foreach(var subFolder in subFolders)
    //  {
    //    var sourceSpecJson = System.IO.Path.Combine(subFolder, Specs.SourceSpec.FileName);

    //    if (_fileSystem.FileExists(sourceSpecJson))
    //    {
    //      var sourceSpec = await _specLoader.LoadSourceSpecAsync(sourceSpecJson);

    //      if (sourceSpec != null)
    //      {
    //        sourceSpecs.Add(sourceSpec);
    //      }
    //    }
    //  }

    //  return sourceSpecs;
    //}

  }
}
