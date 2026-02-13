using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record HierarchyOverrideSpec
  {
    public required string ParentId { get; init; }

    public required IReadOnlyCollection<string> ChildrenIds { get; init; }
  }

}
