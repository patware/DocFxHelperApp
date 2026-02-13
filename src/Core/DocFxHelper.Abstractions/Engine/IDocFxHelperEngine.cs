using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions.Engine
{
  public interface IDocFxHelperEngine
  {
    Task<BuildResult> BuildAsync(
        BuildRequest request,
        CancellationToken cancellationToken = default);
  }
}
