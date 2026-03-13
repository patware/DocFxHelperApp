namespace DocFxHelper.Abstractions.Engine
{
  public sealed record BuildPaths
  {
    public required string WorkingDirectory { get; set; }
    public string Drop { get; init; } = "_drop";
    public string Sources { get; init; } = "_sources";
    public string Converted { get; init; } = "_converted";
    public string Staging { get; init; } = "_staging";
    public string Site { get; init; } = "_site";

    public static BuildPaths Default => new() { WorkingDirectory = Environment.CurrentDirectory};

    public static BuildPaths Resolve(string workingDirectory) => new()
    {
      WorkingDirectory = workingDirectory,
      Drop = System.IO.Path.Combine(workingDirectory, Default.Drop),
      Sources = System.IO.Path.Combine(workingDirectory, Default.Sources),
      Converted = System.IO.Path.Combine(workingDirectory, Default.Converted),
      Staging = System.IO.Path.Combine(workingDirectory, Default.Staging),
      Site = System.IO.Path.Combine(workingDirectory, Default.Site)
    };

  }
}
