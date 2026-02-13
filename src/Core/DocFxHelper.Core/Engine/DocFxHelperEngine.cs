using DocFxHelper.Abstractions;
using DocFxHelper.Abstractions.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Engine
{
  public sealed class DocFxHelperEngine : IDocFxHelperEngine
  {
    public DocFxHelperEngine(Abstractions.ITimeService timeService)
    {
      _time = timeService;
    }

    private readonly ITimeService _time;


    public async Task<BuildResult> BuildAsync(BuildRequest request, CancellationToken cancellationToken = default)
    {
      var start = _time.UtcNow;

      // TODO: GraphBuilder

      // TODO: Stage execution

      // TODO: Caching

      await Task.CompletedTask;

      return new BuildResult
      {
        Status = BuildStatus.Succeeded,
        Duration = DateTime.UtcNow - start,
      };
    }
  }
}
