namespace DocFxHelper.Abstractions.Engine
{
  public sealed record SourceDefinition
  {
    public required string Id { get; init; }

    public required string SourceType { get; init; }

    public required string Location { get; init; }

    public IReadOnlyDictionary<string, string> Parameters { get; init; }
        = new Dictionary<string, string>();
  }
}
