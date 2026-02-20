namespace DocFxHelper.Core.Specs
{
  public sealed record SourceSpec
  {
    public required string Id { get; init; }

    public string? ParentId { get; init; }

    public required string SourceType { get; init; }

    public required string TargetPath { get; init; }

    public required string DisplayName { get; init; }

    public string DefaultPage { get; init; } = "default.md";

    public IReadOnlyCollection<string> Tags { get; init; }
        = Array.Empty<string>();

    public MenuPlacementSpec? Menu { get; init; }

    public IReadOnlyCollection<TemplateSpec> Templates { get; init; }
        = Array.Empty<TemplateSpec>();
  }
}
