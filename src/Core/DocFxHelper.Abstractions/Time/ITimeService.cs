using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions.Time
{
  public interface ITimeService
  {
    DateTimeOffset UtcNow { get; }
  }
}
