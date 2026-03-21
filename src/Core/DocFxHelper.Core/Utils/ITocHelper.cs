using YamlDotNet.RepresentationModel;

namespace DocFxHelper.Core.Utils
{
  public interface ITocHelper
  {
    Graph.Toc GetToc(string yaml);

    string GetString(Graph.Toc toc);
  }
}