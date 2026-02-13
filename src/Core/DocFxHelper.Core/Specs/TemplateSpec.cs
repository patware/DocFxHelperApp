using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed record TemplateSpec
  {
    public required string TemplateFile { get; init; }

    public required string OutputFile { get; init; }

    public TemplateScope Scope { get; init; } = TemplateScope.Site;

    public TemplateCondition? Condition { get; init; }
  }

}
