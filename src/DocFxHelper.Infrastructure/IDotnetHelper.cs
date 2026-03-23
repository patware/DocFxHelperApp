namespace DocFxHelper.Infrastructure
{
  public interface IDotnetHelper
  {
    Task<DotnetToolListResult> GetDotnetToolPackageAsync(string package, bool global, CancellationToken ct = default);
    Task<DotnetRunResult> RunTool(string workingDirectory, string command,IReadOnlyList<string> arguments);
  }
}