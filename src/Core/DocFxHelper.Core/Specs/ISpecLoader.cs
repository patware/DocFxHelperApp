using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public interface ISpecLoader
  {
    Task<MasterSpec> LoadMasterSpecAsync(string path, CancellationToken ct = default!);
    Task<SourceSpec> LoadSourceSpecAsync(string path, CancellationToken ct = default!);
    Task<BuildSpec> LoadBuildSpecAsync(string path, CancellationToken ct = default!);
  }
}
