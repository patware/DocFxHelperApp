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


      if (!_fileSystem.DirectoryExists(buildPaths.Orphans))
      {
        _logger.LogInformation("  Orphans folder not found, creating [{folder}]", buildPaths.Orphans);
        _fileSystem.CreateDirectory(buildPaths.Orphans);
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

    public void MoveToOrphanFolder(string folderPath, BuildPaths buildPaths)
    {

      var di = new DirectoryInfo(folderPath);
      var orphanFolder = System.IO.Path.Combine(buildPaths.Orphans, string.Concat(di.Name, "_", DateTime.Now.ToString("yyyymmdd_hhMMss")));

      _logger.LogInformation("Moving [{from}] to [{to}]", System.IO.Path.GetRelativePath(buildPaths.WorkingDirectory, folderPath), System.IO.Path.GetRelativePath(buildPaths.WorkingDirectory, orphanFolder));
      _fileSystem.MoveDirectory(folderPath, orphanFolder);
    }
  }
}
