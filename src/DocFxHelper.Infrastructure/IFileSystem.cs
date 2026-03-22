using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public interface IFileSystem
  {
    IReadOnlyList<string> GetDirectories(string path, bool recurse);

    bool DirectoryExists(string path);
    void CreateDirectory(string path);
    void DeleteDirectory(string path);
    void MoveDirectory(string source, string destination);
    void CopyDirectory(string source, string destination);
    string RenameDirectory(string sourceFolder, string destinationFolder);

    IReadOnlyList<string> GetFiles(string folder, string? filter, bool recurse);
    bool FileExists(string path);
    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct = default);

    Task WriteAllTextAsync(string path,string content, CancellationToken ct = default);
    string RenameFile(string sourceFilename, string destinationFilename);
    bool FolderExists(string folder);
    void DeleteFile(string file);
    string? FindFileUpwards(string folder, string filename, int levels = 0);
    void MoveFile(string source, string destination);
    
  }
}
