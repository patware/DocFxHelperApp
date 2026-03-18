using DocFxHelper.Core.Specs;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocFxHelper.Core.Specs
{
  public class JsonSpecLoader : ISpecLoader
  {
    private readonly ILogger<JsonSpecLoader> _logger;
    private readonly IFileSystem _fileSystem;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonSpecLoader(ILogger<JsonSpecLoader> logger, IFileSystem fileSystem)
    {
      _logger = logger;
      _fileSystem = fileSystem;

      _jsonSerializerOptions = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
      };
    }

    public async Task<MasterSpec> LoadMasterSpecAsync(string path, CancellationToken ct = default!)
    {
      if (_fileSystem.FileExists(path))
      {
        _logger.LogInformation("Loading MasterSpec from [{path}]", path);
        var json = await _fileSystem.ReadAllTextAsync(path,ct);

        var masterSpec = System.Text.Json.JsonSerializer.Deserialize<MasterSpec>(json, _jsonSerializerOptions);

        return masterSpec ?? throw new InvalidOperationException($"Failed to deserialize MasterSpec from {path}");
      }
      else
      {
        throw new System.IO.FileNotFoundException($"MasterSpec file not found at path: {path}");
      }

    }


    public async Task<SourceSpec> LoadSourceSpecAsync(string path, CancellationToken ct = default!)
    {
      if (_fileSystem.FileExists(path))
      {
        _logger.LogInformation("Loading SourceSpec from [{path}]", path);
        var json = await _fileSystem.ReadAllTextAsync(path, ct);

        var sourceSpec = System.Text.Json.JsonSerializer.Deserialize<SourceSpec>(json, _jsonSerializerOptions);

        return sourceSpec ?? throw new InvalidOperationException($"Failed to deserialize SourceSpec from {path}");
      }
      else
      {
        throw new System.IO.FileNotFoundException($"SourceSpec file not found at path: {path}");
      }
    }

    public async Task<BuildSpec> LoadBuildSpecAsync(string path, CancellationToken ct = default!)
    {
      if (_fileSystem.FileExists(path))
      {
        _logger.LogInformation("Loading BuildSpec from [{path}]", path);
        var json = await _fileSystem.ReadAllTextAsync(path, ct);

        var buildSpec = System.Text.Json.JsonSerializer.Deserialize<BuildSpec>(json, _jsonSerializerOptions);

        return buildSpec ?? throw new InvalidOperationException($"Failed to deserialize BuildSpec from {path}");
      }
      else
      {
        throw new System.IO.FileNotFoundException($"BuildSpec file not found at path: {path}");
      }
    }

  }
}
