using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Utils
{
  public interface IVerification
  {
    IReadOnlyList<string> GetOrphanFolders(BuildPaths buildPaths, GraphBuildContext graphBuildContext);

    Task<bool> IsDocfxInstalledAsync(CancellationToken ct = default!);
  }
}
