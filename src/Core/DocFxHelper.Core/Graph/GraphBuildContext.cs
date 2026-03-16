using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Graph
{
  public sealed record GraphBuildContext
  {
    public required MasterSpec Master { get; init; }
    public required IReadOnlyDictionary<string,SourceSpec> Sources { get; init; }

    public static GraphBuildContext Default => new()
    {
      Master = default!,
      Sources = new Dictionary<string, SourceSpec>(),
    };
    
  }
}
