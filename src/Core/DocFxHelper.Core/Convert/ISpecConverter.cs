using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public interface ISpecConverter
  {
    Type SourceType { get; }
    Task<int> ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, Specs.SourceSpec sourceSpec, BuildSpec buildSpec, CancellationToken ct = default);
  }
}
