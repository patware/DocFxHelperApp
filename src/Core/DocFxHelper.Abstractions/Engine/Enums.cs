using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Abstractions.Engine
{
  public enum BuildMode
  {
    Incremental,
    Full
  }

  public enum BuildStatus
  {
    Succeeded,
    Failed
  }
}
