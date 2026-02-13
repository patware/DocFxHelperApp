using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions.Engine
{
  public sealed record BuildResult
  {
    public required BuildStatus Status { get; init; }

    public TimeSpan Duration { get; init; }

    public IReadOnlyList<string> Diagnostics { get; init; } = [];
  }
}
