using DocFxHelper.Core.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public interface ISpecLoader
  {
    Task<MasterSpec> LoadMasterSpecAsync(string path);
    Task<SourceSpec> LoadSourceSpecAsync(string path);
    Task<BuildSpec> LoadBuildSpecAsync(string path);
    Task<TemplateSpec> LoadTemplateSpecAsync(string path);
  }
}
