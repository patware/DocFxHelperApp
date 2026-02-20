namespace DocFxHelper.Core.Specs
{
  public sealed record RunSpec
  {
    public required string BuildId { get; init; }

    public required DateTimeOffset Timestamp { get; init; }

    public required string EngineVersion { get; init; }

    public IReadOnlyDictionary<string, string> SourceVersions { get; init; }
        = new Dictionary<string, string>();

    public IReadOnlyDictionary<string, string> Metadata { get; init; }
        = new Dictionary<string, string>();
  }
}
