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




    public void DeleteDirectory(string path)
    {
      System.IO.Directory.Delete(path, recursive: true);
    }

    public void MoveDirectory(string source, string destination)
    {
      System.IO.Directory.Move(source, destination);
    }

    public void CopyDirectory(string source, string destination)
    {
      foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
      {
        Directory.CreateDirectory(dirPath.Replace(source, destination));
      }

      foreach (var filePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
      {
        File.Copy(filePath, filePath.Replace(source, destination), true);
      }


    }

    public void CreateDirectory(string path)
    {
      System.IO.Directory.CreateDirectory(path);
    }

    public bool FileExists(string path)
        => File.Exists(path);


    public IReadOnlyList<string> GetFiles(string folder, string? filter)
    {
      return System.IO.Directory.GetFiles(folder, filter ?? "*", SearchOption.AllDirectories);
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken ct = default)
        => await File.ReadAllTextAsync(path, ct);

    public async Task WriteAllTextAsync(string path, string content, CancellationToken ct = default)
      => await File.WriteAllTextAsync(path, content, ct);
  }
}
