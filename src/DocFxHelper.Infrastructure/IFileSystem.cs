using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public interface IFileSystem
  {
    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    bool FileExists(string path);
  }
}
