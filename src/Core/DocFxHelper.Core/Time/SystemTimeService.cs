using DocFxHelper.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Time
{
  public sealed class SystemTimeService : ITimeService
  {
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
  }
}
