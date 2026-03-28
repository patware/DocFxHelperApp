using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Building
{
  public interface IDocFxBuildSourceContributor
  {
    bool CanHandle(SourceSpec sourceSpec);
    DocFxBuildFragment Create(SiteNode node);
  }
}
