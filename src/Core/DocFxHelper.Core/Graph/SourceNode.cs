using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public sealed record SourceNode
  {
    public required string SourceId { get; init; }

    public required string SourceType { get; init; }

    public required string RootPath { get; init; }

    public IReadOnlyDictionary<string, string> Tags { get; init; }
        = new Dictionary<string, string>();
  }
}
