using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Graph
{
  public record SiteNode
  {
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Path { get; init; }
    public IReadOnlyList<SiteNode> Children { get; init; } = new List<SiteNode>();

    public required SourceSpec SourceSpec { get; init; }
  }
}
