using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Utils
{
  public class Verification(ILogger<Verification> logger, IFileSystem fileSystem) : IVerification
  {
    private readonly ILogger<Verification> _logger = logger;
    private readonly IFileSystem _fileSystem = fileSystem;

    public IReadOnlyList<string> GetOrphanFolders(BuildPaths buildPaths, GraphBuildContext graphBuildContext)
    {
      _logger.LogInformation("Checking {source} for orphan sources", buildPaths.Sources);

      var orphans = new List<string>();

      var validIds = graphBuildContext.Sources.Keys.ToList();

      var foldersToCleanup = new List<string>();

      foldersToCleanup.AddRange(_fileSystem.GetDirectories(buildPaths.Sources));
      foldersToCleanup.AddRange(_fileSystem.GetDirectories(buildPaths.Converted));
      foldersToCleanup.AddRange(_fileSystem.GetDirectories(buildPaths.Staging));

      foreach (var sourceFolder in foldersToCleanup)
      {
        var di = new DirectoryInfo(sourceFolder);

        if (!validIds.Contains(di.Name) && di.Name != "_master")
        {
          orphans.Add(di.FullName);
        }
      }

      return orphans;
    }
  }
}
