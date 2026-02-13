using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public sealed record SiteGraph
  {
    public required SiteNode Root { get; init; }

    public required IReadOnlyDictionary<string, SourceNode> Sources { get; init; }

    public required GraphBuildContext Build { get; init; }
  }
}
