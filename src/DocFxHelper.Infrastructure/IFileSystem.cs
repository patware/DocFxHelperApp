using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public interface IFileSystem
  {
    IReadOnlyList<string> GetDirectories(string folder);

    bool FileExists(string path);

    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    bool DirectoryExists(string drop);
  }
}
