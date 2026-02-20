namespace DocFxHelper.Core.Specs
{
  public sealed record MasterSpec
  {
    public required string RootPath { get; init; }

    public IReadOnlyCollection<HierarchyOverrideSpec> HierarchyOverrides { get; init; }
        = Array.Empty<HierarchyOverrideSpec>();

    public IReadOnlyCollection<SourceOverrideSpec> SourceOverrides { get; init; }
        = Array.Empty<SourceOverrideSpec>();

    public IReadOnlyCollection<string> DisabledSources { get; init; }
        = Array.Empty<string>();
  }

  public sealed record HierarchyOverrideSpec
  {
    public required string ParentId { get; init; }
    public required IReadOnlyCollection<string> ChildrenIds { get; init; }
  }

  public sealed record SourceOverrideSpec
  {
    public required string SourceId { get; init; }

    public string? ParentId { get; init; }

    public string? TargetPath { get; init; }

    public string? DisplayName { get; init; }

    public int? Order { get; init; }
  }
}
