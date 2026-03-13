using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public interface ISpecConverter
  {
    Type SourceType { get; }
    Task ConvertAsync(Specs.SourceSpec sourceSpec);
  }
}
