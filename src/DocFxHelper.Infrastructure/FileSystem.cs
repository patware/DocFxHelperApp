using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public class FileSystem : IFileSystem
  {
    public bool DirectoryExists(string path)
        => Directory.Exists(path);

    public IReadOnlyList<string> GetDirectories(string path)
    {
      if (DirectoryExists(path))
      {
        return Directory.GetDirectories(path);
      }

      return [];
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken ct = default)
        => await File.ReadAllTextAsync(path, ct);

    public bool FileExists(string path)
        => File.Exists(path);

    public void DeleteDirectory(string path)
    {
      System.IO.Directory.Delete(path, recursive: true);
    }

    public void MoveDirectory(string itemPath, string destinationFolder)
    {
      System.IO.Directory.Move(itemPath, destinationFolder);
    }

    public void CreateDirectory(string path)
    {
      System.IO.Directory.CreateDirectory(path);
    }
  }
}
