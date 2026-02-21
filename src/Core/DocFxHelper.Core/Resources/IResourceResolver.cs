using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Resources
{
  public interface IResourceResolver
  {
    ResolvedResource Resolve(string node, string baseDirectory);
  }
}
