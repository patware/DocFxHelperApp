using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public sealed record PageNode : SiteNode
  {
    public required PageKind Kind { get; init; }

    public required ContentReference Content { get; init; }

  }
}
