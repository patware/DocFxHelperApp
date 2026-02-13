using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public sealed record GraphBuildContext
  {
    public required IReadOnlyList<SourceSpec> Sources { get; init; }
    public required MasterSpec Master { get; init; }
    public required RunSpec Run { get; init; }
  }
}
