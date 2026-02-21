using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocFxHelper.Infrastructure.SpecLoading
{
  public class JsonSpecLoader : ISpecLoader
  {
    private IFileSystem _fileSystem;
    private JsonSerializerOptions _jsonSerializerOptions;

    public JsonSpecLoader(IFileSystem fileSystem)
    {
      _fileSystem = fileSystem;

      _jsonSerializerOptions = new JsonSerializerOptions
      {
        Converters = { new JsonStringEnumConverter() }
      };
    }

    public async Task<MasterSpec> LoadMasterSpecAsync(string path)
    {
      if (_fileSystem.FileExists(path))
      {
        var json = await _fileSystem.ReadAllTextAsync(path);

        var masterSpec = System.Text.Json.JsonSerializer.Deserialize<MasterSpec>(json, _jsonSerializerOptions);

        return masterSpec ?? throw new InvalidOperationException($"Failed to deserialize MasterSpec from {path}");
      }
      else
      {
        throw new System.IO.FileNotFoundException($"MasterSpec file not found at path: {path}");
      }

    }
        

    public async Task<SourceSpec> LoadSourceSpecAsync(string path)
    {
      if (_fileSystem.FileExists(path))
      {
        var json = await _fileSystem.ReadAllTextAsync(path);

        var sourceSpec = System.Text.Json.JsonSerializer.Deserialize<SourceSpec>(json, _jsonSerializerOptions);

        return sourceSpec ?? throw new InvalidOperationException($"Failed to deserialize SourceSpec from {path}");
      }
      else
      {
        throw new System.IO.FileNotFoundException($"SourceSpec file not found at path: {path}");
      }
    }
        
  }
}
