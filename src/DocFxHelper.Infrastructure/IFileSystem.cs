using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public interface IFileSystem
  {
    IReadOnlyList<string> GetDirectories(string path);

    bool FileExists(string path);

    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    bool DirectoryExists(string path);
    void CreateDirectory(string path);
    void DeleteDirectory(string path);
    void MoveDirectory(string itemPath, string destinationFolder);
  }
}
