using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Domain
{
  public record RunTrigger
  {
    public required string Source { get; init; }
    public required string Author { get; init; }


  }
}
