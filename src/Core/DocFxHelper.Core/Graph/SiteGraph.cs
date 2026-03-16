namespace DocFxHelper.Core.Graph
{
  public sealed record SiteGraph
  {
    public required SiteNode Root { get; init; }

    public required IReadOnlyDictionary<string, SiteNode> Sources { get; init; }

    public required GraphBuildContext BuildContext { get; init; }

    public static SiteGraph Default = new()
    {
      Root = default!,
      Sources = new Dictionary<string, SiteNode>(),
      BuildContext = default!
    };
  }
}
