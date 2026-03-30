using DocFxHelper.Core.Specs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public abstract class BaseConverter<T> : ISpecConverter where T : Specs.SourceSpec
  {
    public Type SourceType => typeof(T);

    public async Task<int> ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, SourceSpec sourceSpec, BuildSpec buildSpec, CancellationToken ct = default)
      
    {
      return await Convert(buildPaths, (T)sourceSpec, buildSpec, ct);
    }

    public abstract Task<int> Convert(Abstractions.Engine.BuildPaths buildPaths, T sourceSpec, BuildSpec buildSpec, CancellationToken ct = default);
  }
}
