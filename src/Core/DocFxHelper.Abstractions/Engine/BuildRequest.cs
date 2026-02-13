using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions.Engine
{
  public sealed record BuildRequest
  {
    public required IReadOnlyList<SourceDefinition> Sources { get; init; }

    public required BuildMode Mode { get; init; }

    public BuildPaths Paths { get; init; } = BuildPaths.Default;
  }
}
