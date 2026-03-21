using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public class TocItem
  {
    public string? Name { get; set; }
    public string? Href { get; set; }
    public string? Homepage { get; set; }
    public List<TocItem>? Items { get; set; }
  }
}
