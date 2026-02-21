using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Resources
{
  public class ResolvedResource
  {
    public required string PhysicalPath { get; init; }
    public required string OriginalNode { get; init; }
  }
}
