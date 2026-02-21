using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure.SpecLoading
{
  public interface ISpecLoader
  {
    Task<MasterSpec> LoadMasterSpecAsync(string path);
    Task<SourceSpec> LoadSourceSpecAsync(string path);
  }
}
