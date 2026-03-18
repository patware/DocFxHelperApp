using DocFxHelper.Abstractions.Engine;

namespace DocFxHelper.Core.Building
{
  public interface IAssembly
  {
    Task Assemble(BuildPaths buildPaths, Graph.SiteGraph siteGraph, CancellationToken ct = default!);
  }
}