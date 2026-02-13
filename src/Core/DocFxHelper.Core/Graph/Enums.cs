using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  public enum PageKind
  {
    Static,      // Source file
    Generated,   // Mustache output
    Index,       // Section default
  }
}
