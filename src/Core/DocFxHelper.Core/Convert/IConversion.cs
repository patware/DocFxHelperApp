using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public interface IConversion
  {
    public Task ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, Graph.SiteNode siteNode, CancellationToken ct = default!);
  }
}
