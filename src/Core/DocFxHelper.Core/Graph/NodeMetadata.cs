using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public sealed record NodeMetadata
  {
    public bool HiddenFromMenu { get; init; }

    public int? Order { get; init; }

    public string? DefaultChildId { get; init; }

    public IReadOnlyDictionary<string, string> Custom { get; init; }
        = new Dictionary<string, string>();
  }

}
