using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record TemplateCondition
  {
    public string? RequiredTag { get; init; }

    public string? RequiredSourceType { get; init; }
  }

}
