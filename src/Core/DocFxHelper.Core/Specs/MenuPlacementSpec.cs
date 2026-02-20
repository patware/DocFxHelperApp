namespace DocFxHelper.Core.Specs
{
  public sealed record MenuPlacementSpec
  {
    public int? Order { get; init; }

    public string? InsertAfterId { get; init; }

    public bool SortAlphabetically { get; init; }
  }
}
