using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public interface IConversion
  {
    public Task ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, Specs.SourceSpec sourceSpec, CancellationToken ct = default!);
  }
}
