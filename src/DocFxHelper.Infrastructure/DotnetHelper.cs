using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DocFxHelper.Infrastructure
{
  public class DotnetHelper : IDotnetHelper
  {
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      Converters = { new JsonStringEnumConverter() }
    };

    public async Task<DotnetToolListResult> GetDotnetToolPackageAsync(string package, bool global, CancellationToken ct = default)
    {
      var pi = new System.Diagnostics.ProcessStartInfo
      {
        FileName = "dotnet",                          // run dotnet
        RedirectStandardOutput = true,                // Redirect output to capture it
        RedirectStandardError = true,                 // Redirect error output
        UseShellExecute = false,                      // Do not use the operating system shell
        CreateNoWindow = true                         // Do not create a visible window
      };

      pi.ArgumentList.Add("tool");
      pi.ArgumentList.Add("list");
      pi.ArgumentList.Add("docfx");
      pi.ArgumentList.Add("--format");
      pi.ArgumentList.Add("json");

      if (global)
      {
        pi.ArgumentList.Add("--global");
      }

      var process = new System.Diagnostics.Process()
      {
        StartInfo = pi
      };

      try
      {
        process.Start();

        // Read the output and error asynchronously to prevent deadlocks
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var result = JsonSerializer.Deserialize<DotnetToolListResult>(output, _jsonSerializerOptions);

        return result!;

      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");
      }

      return new DotnetToolListResult
      {
        Data = []
      };

    }

    public async Task<DotnetRunResult> RunTool(string workingDirectory, string command, IReadOnlyList<string> arguments)
    {
      var processInfo = new ProcessStartInfo
      {
        FileName = command,                          // run dotnet
        RedirectStandardOutput = true,                // Redirect output to capture it
        RedirectStandardError = true,                 // Redirect error output
        UseShellExecute = false,                      // Do not use the operating system shell
        CreateNoWindow = true,                         // Do not create a visible window
        WorkingDirectory = workingDirectory
      };

      foreach(var arg in arguments)
      {
        processInfo.ArgumentList.Add(arg);
      }

      var process = new System.Diagnostics.Process()
      {
        StartInfo = processInfo
      };

      try
      {
        process.Start();

        // Read the output and error asynchronously to prevent deadlocks
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var result = new DotnetRunResult
        {
          ExitCode = process.ExitCode,
          Output = output,
          Error = error
        };

        return result;

      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred: {ex.Message}");

        var result = new DotnetRunResult()
        {
          ExitCode = -1,
          Output = "process.Start() exception",
          Error = ex.ToString()
        };

        return result;
      }

    }
  }

  public class DotnetRunResult
  {
    public required int ExitCode { get; set; }
    public required string Output { get; set; }
    public required string Error { get; set; }

  }

  public class DotnetToolListResult
  {
    public int Version { get; set; }
    public required IReadOnlyList<DotnetToolListItem> Data { get; set; }
  }

  public class DotnetToolListItem
  {
    public required string PackageId { get; set; }

    public required string Version { get; set; }

    public required IReadOnlyList<string> Commands { get; set; }
  
  }
}
