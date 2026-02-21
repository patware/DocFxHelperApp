using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Resources
{
  public class LocalResourceResolver : IResourceResolver
  {
    public ResolvedResource Resolve(string node, string baseDirectory)
    {
      string resolvedPath = Path.IsPathRooted(node)
          ? node
          : Path.GetFullPath(Path.Combine(baseDirectory, node));

      if (!Directory.Exists(resolvedPath))
        throw new DirectoryNotFoundException(
            $"Resource '{node}' resolved to '{resolvedPath}' but does not exist.");

      return new ResolvedResource
      {
        OriginalNode = node,
        PhysicalPath = resolvedPath
      };
    }
  }
}
