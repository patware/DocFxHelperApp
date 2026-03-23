using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure
{
  public class DocFxInstallationMetadata
  {
    public required string Version { get; init; }
    public required bool IsGlobal { get; init; }

    public required string Command { get; init; }
    

  }
}
