using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public class FileSystem : IFileSystem
  {
    public bool DirectoryExists(string path)
        => Directory.Exists(path);

    public IReadOnlyList<string> GetDirectories(string folder)
    {
      if (DirectoryExists(folder))
      {
        return Directory.GetDirectories(folder);
      }

      return [];
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken ct = default)
        => await File.ReadAllTextAsync(path, ct);

    public bool FileExists(string path)
        => File.Exists(path);

    
  }
}
