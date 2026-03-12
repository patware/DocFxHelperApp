using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Drops
{
  public class Scout(DocFxHelper.Infrastructure.IFileSystem fileSystem) : IScout
  {

    private readonly DocFxHelper.Infrastructure.IFileSystem _fileSystem = fileSystem;

    public IReadOnlyList<System.IO.DirectoryInfo> Recon(string drop)
    {
      var readySources = new List<System.IO.DirectoryInfo>();

      foreach (var source in _fileSystem.GetDirectories(drop))
      {
        var sourceSpecJson = System.IO.Path.Combine(source, "source.spec.json");
        var masterSpecJson = System.IO.Path.Combine(source, "master.spec.json");
        var buildSpecJson = System.IO.Path.Combine(source, "build.spec.json");

        if ((_fileSystem.FileExists(sourceSpecJson) || _fileSystem.FileExists(masterSpecJson)) && _fileSystem.FileExists(buildSpecJson))
        {
          readySources.Add(new DirectoryInfo(source));
        }
      }

      return readySources;
    }
  }
}
