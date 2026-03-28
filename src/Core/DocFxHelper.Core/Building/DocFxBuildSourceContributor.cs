using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Building
{
  public abstract class DocFxBuildSourceContributor<TSourceSpec> : IDocFxBuildSourceContributor where TSourceSpec : SourceSpec
  {
    public bool CanHandle(SourceSpec sourceSpec) => sourceSpec is TSourceSpec;

    public DocFxBuildFragment Create(SiteNode node)
    {
      return Create(node, (TSourceSpec)node.SourceSpec);
    }

    protected abstract DocFxBuildFragment Create(SiteNode node, TSourceSpec sourceSpec);
  }
}
