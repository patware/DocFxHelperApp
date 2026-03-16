using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Graph
{
  public interface IGraphBuilder
  {
    Task<SiteGraph> BuildAsync(string path);
  }
}
