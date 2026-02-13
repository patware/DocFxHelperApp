using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Graph
{
  internal interface IGraphBuilder
  {
    SiteGraph Build(
            IReadOnlyList<Specs.SourceSpec> sources,
            Specs.MasterSpec master,
            Specs.RunSpec run);
  }
}
