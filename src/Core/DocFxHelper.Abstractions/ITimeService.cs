using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions
{
  public interface ITimeService
  {
    DateTimeOffset UtcNow { get; }
  }
}
