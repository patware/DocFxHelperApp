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

    public async Task ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, SourceSpec sourceSpec, CancellationToken ct = default)
    {
      await Convert(buildPaths, (T)sourceSpec, ct);
    }

    public abstract Task Convert(Abstractions.Engine.BuildPaths buildPaths, T sourceSpec, CancellationToken ct = default);
  }
}
