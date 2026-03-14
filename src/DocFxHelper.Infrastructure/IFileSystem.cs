using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public interface IFileSystem
  {
    IReadOnlyList<string> GetDirectories(string path);

    bool DirectoryExists(string path);
    void CreateDirectory(string path);
    void DeleteDirectory(string path);
    void MoveDirectory(string source, string destination);
    void CopyDirectory(string source, string destination);

    IReadOnlyList<string> GetFiles(string folder, string? filter);
    bool FileExists(string path);
    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    Task WriteAllTextAsync(string path,string content, CancellationToken ct = default);
  }
}
