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

    public async Task ConvertAsync(SourceSpec sourceSpec)
    {
      await Convert((T)sourceSpec);
    }

    public abstract Task Convert(T sourceSpec);
  }
}
