using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record MasterSpec
  {
    public required string RootPath { get; init; }

    public IReadOnlyCollection<HierarchyOverrideSpec> HierarchyOverrides { get; init; }
        = Array.Empty<HierarchyOverrideSpec>();

    public IReadOnlyCollection<SourceOverrideSpec> SourceOverrides { get; init; }
        = Array.Empty<SourceOverrideSpec>();

    public IReadOnlyCollection<string> DisabledSources { get; init; }
        = Array.Empty<string>();
  }
}
