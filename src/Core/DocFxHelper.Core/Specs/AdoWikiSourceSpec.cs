using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed class AdoWikiSourceSpec : SourceSpec
  {
    public required string WikiUrl { get; init; }
  }
}
