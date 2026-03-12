using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Graph
{
  internal interface IGraphBuilder
  {
    SiteGraph Build(
      IReadOnlyList<SourceSpec> sources,
      Specs.MasterSpec master
      );
  }
}
