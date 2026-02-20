namespace DocFxHelper.Core.Graph
{
  public sealed record ContentReference
  {
    public required string RelativePath { get; init; }

    public required string Hash { get; init; }
  }

}
