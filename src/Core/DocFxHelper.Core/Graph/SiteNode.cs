using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Graph
{
  public record SiteNode
  {
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Path { get; init; }
    public required bool ShowInToc { get; init; }
    public int? TocItemInsertAtIndex { get; init; }
    public string? ParentTocDisplayName { get; init; }
    public IReadOnlyList<SiteNode> Children { get; init; } = [];

    public required SourceSpec SourceSpec { get; init; }
    public required BuildSpec BuildSpec { get; init; }
    public required string DefaultPage { get; init; }
  }
}
