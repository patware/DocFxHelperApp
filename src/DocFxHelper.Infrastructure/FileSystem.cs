using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public class FileSystem(ILogger<FileSystem> logger) : IFileSystem
  {
    private readonly ILogger<FileSystem> _logger = logger;

    public bool DirectoryExists(string path)
        => Directory.Exists(path);

    public IReadOnlyList<string> GetDirectories(string path, bool recurse)
    {
      SearchOption searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

      if (DirectoryExists(path))
      {
        _logger.LogDebug("GetDirectories: path {path} exists", path);
        return Directory.GetDirectories(path, "*", searchOption);
      }

      _logger.LogDebug("GetDirectories: path {path} does not exist, return null", path);
      return [];
    }




    public void DeleteDirectory(string path)
    {
      _logger.LogDebug("Deleting directory {path}", path);
      System.IO.Directory.Delete(path, recursive: true);
    }

    public void MoveDirectory(string source, string destination)
    {
      _logger.LogDebug("Moving directory from {source} to {destination}", source, destination);
      System.IO.Directory.Move(source, destination);
    }

    public void CopyDirectory(string source, string destination, IReadOnlyList<string>? excludeFiles)
    {
      _logger.LogDebug("Copying directory from {source} to {destination}", source, destination);
      Directory.CreateDirectory(destination);

      foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
      {
        var ftc = dirPath.Replace(source, destination);
        _logger.LogDebug("Creating sub directory {directory}", ftc);
        Directory.CreateDirectory(ftc);
      }

      foreach (var filePath in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
      {
        var fn = Path.GetFileName(filePath);

        if (excludeFiles!=null && excludeFiles.Any(n => string.Equals(n, fn)))
        {
          _logger.LogDebug("File {filePath} excluded from copy", filePath);
        }
        else
        {
          _logger.LogDebug("Copy File {filePath}", filePath);
          System.IO.File.Copy(filePath, filePath.Replace(source, destination), true);

        }
      }


    }

    public void CreateDirectory(string path)
      => Directory.CreateDirectory(path);


    public bool FolderExists(string folder)
      => Directory.Exists(folder);


    public bool FileExists(string path)
        => File.Exists(path);


    public IReadOnlyList<string> GetFiles(string folder, string? filter, bool recurse)
    {
      SearchOption options;

      if (recurse)
      {
        options = SearchOption.AllDirectories;
      }
      else
      {
        options = SearchOption.TopDirectoryOnly;
      }

      return System.IO.Directory.GetFiles(folder, filter ?? "*", options);
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken ct = default)
        => await File.ReadAllTextAsync(path, ct);

    public async Task WriteAllTextAsync(string path, string content, CancellationToken ct = default)
      => await File.WriteAllTextAsync(path, content, ct);

    public async Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct = default)
      => await File.ReadAllLinesAsync(path, ct);

    public string RenameFile(string sourceFilename, string destinationFilename)
    {
      var newPath = Path.Combine(Path.GetDirectoryName(sourceFilename)!, destinationFilename);

      File.Move(sourceFilename, newPath);

      return newPath;
    }

    public void DeleteFile(string file)
      => File.Delete(file);

    public string? FindFileUpwards(string path, string filename, int levels = 0)
    {
      var di = new DirectoryInfo(path);
            
      do
      {
        var pathToFile = System.IO.Path.Combine(di.FullName, filename);
        if (File.Exists(pathToFile))
        {
          return pathToFile;
        }

        di = di.Parent;

      }while(di != null && di.FullName.Split(Path.DirectorySeparatorChar).Length >= levels);

      return null;
    }

    public void MoveFile(string source, string destination)
      => System.IO.File.Move(source, destination);

    public string RenameDirectory(string sourceFolder, string destinationFolder)
    {
      var newPath = Path.Combine(Path.GetDirectoryName(sourceFolder)!, destinationFolder);

      Directory.Move(sourceFolder, newPath);

      return newPath;

    }
  }
}
