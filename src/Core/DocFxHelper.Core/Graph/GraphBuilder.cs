using DocFxHelper.Core.Resources;
using DocFxHelper.Core.Specs;
using System.Collections.ObjectModel;

namespace DocFxHelper.Core.Graph
{
  public class GraphBuilder : IGraphBuilder
  {

    public SiteGraph Build(IReadOnlyList<SourceSpec> sources, MasterSpec master)
    {

      var sourcesDictionary = new Dictionary<string, SourceNode>();

      var siteGraph = new SiteGraph
      {
        // Provide a placeholder root. Replace with a real SiteNode when available.
        Root = default!,

        // Create an empty dictionary for Sources. Populate this with real SourceNode instances when mapping is implemented.
        Sources = sourcesDictionary,

        // Provide a placeholder build context. Replace with a real GraphBuildContext when available.
        Build = default!
      };

      return siteGraph;
    }

  }
}
