using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Specs;
using DocFxHelper.Core.Utils;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public class RestApiConverter(
    ILogger<RestApiConverter> logger,
    Infrastructure.IFileSystem fileSystem,
    Utils.ITocHelper tocHelper) : BaseConverter<Specs.RestApiSourceSpec>
  {
    private readonly ILogger<RestApiConverter> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;
    private readonly Utils.ITocHelper _tocHelper = tocHelper;

    public override async Task<int> Convert(BuildPaths buildPaths, RestApiSourceSpec sourceSpec, CancellationToken ct = default)
    {
      _logger.LogInformation("---------------");
      _logger.LogInformation("REST Api [{id}] Conversion started", sourceSpec.Id);

      _logger.LogInformation("Task 1 Generate blank toc.yml");

      var yaml = """
        items:
        """;

      var sourcePath = Path.Combine(buildPaths.Sources, sourceSpec.Id, "toc.yml");
      if (_fileSystem.FileExists(sourcePath))
      {
        _fileSystem.DeleteFile(sourcePath);
      }

      await _fileSystem.WriteAllTextAsync(sourcePath, yaml, ct);

      _logger.LogInformation("REST Api [{id}] Conversion ended", sourceSpec.Id);

      return 1;
    }
  }
}
