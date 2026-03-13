using DocFxHelper.Abstractions.Engine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Utils
{
  public class General(
    ILogger<General> logger,
    Infrastructure.IFileSystem fileSystem) : IGeneral
  {
    private readonly ILogger<General> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;

    public void EnsureExists(BuildPaths buildPaths)
    {
      _logger.LogInformation("Checking if [{workingDirectory}] has all the expected sub folders", buildPaths.WorkingDirectory);

      if (!_fileSystem.DirectoryExists(buildPaths.Drop))
      {
        _logger.LogInformation("  Drop folder not found, creating [{folder}]", buildPaths.Drop);
        _fileSystem.CreateDirectory(buildPaths.Drop);
      }

      if (!_fileSystem.DirectoryExists(buildPaths.Sources))
      {
        _logger.LogInformation("  Sources folder not found, creating [{folder}]", buildPaths.Sources);
        _fileSystem.CreateDirectory(buildPaths.Sources);
      }

      if (!_fileSystem.DirectoryExists(buildPaths.Converted))
      {
        _logger.LogInformation("  Converted folder not found, creating [{folder}]", buildPaths.Converted);
        _fileSystem.CreateDirectory(buildPaths.Converted);
      }

      if (!_fileSystem.DirectoryExists(buildPaths.Staging))
      {
        _logger.LogInformation("  Staging folder not found, creating [{folder}]", buildPaths.Staging);
        _fileSystem.CreateDirectory(buildPaths.Staging);
      }

      if (!_fileSystem.DirectoryExists(buildPaths.Site))
      {
        _logger.LogInformation("  Site folder folder not found, creating [{folder}]", buildPaths.Site);
        _fileSystem.CreateDirectory(buildPaths.Site);
      }
    }
  }
}
