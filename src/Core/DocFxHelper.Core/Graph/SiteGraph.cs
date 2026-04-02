using DocFxHelper.Infrastructure;

namespace DocFxHelper.Core.Graph
{
  public class SiteGraph : HierarchyDictionary<string, SiteNode>
  {
    //public required SiteNode Root { get; init; }

    //public required IDictionary<string, SiteNode> Sources { get; init; }

    public required GraphBuildContext BuildContext { get; init; }

    public static SiteGraph Default = new()
    {
      //Root = default!,
      //Sources = new Dictionary<string, SiteNode>(),
      BuildContext = default!
    };
  }
}
