using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using System.Text.Json.Nodes;

namespace DocFxHelper.Core.Building
{
  public interface IAssemblyHelper
  {
    void CleanStaging(BuildPaths buildPaths);
    void CopySourceToStaging(SiteNode siteNode, BuildPaths buildPaths);
    Task<JsonNode> GenerateDocFxConfigAsync(BuildPaths buildPaths, SiteGraph siteGraph, CancellationToken ct = default);
    Task LinkChildSourcesToParent(BuildPaths buildPaths, SiteGraph siteGraph, CancellationToken ct = default);
    void ProcessMustacheTemplates(BuildPaths buildPaths, SiteGraph siteGraph);
    Task SaveDocFxJsonAsync(BuildPaths buildPaths, JsonNode docFx, CancellationToken ct = default);
  }
}