using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record SourceOverrideSpec
  {
    public required string SourceId { get; init; }

    public string? ParentId { get; init; }

    public string? TargetPath { get; init; }

    public string? DisplayName { get; init; }

    public int? Order { get; init; }
  }

}
