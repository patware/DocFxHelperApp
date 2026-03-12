using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Sources
{
  public interface IIngestion
  {
    Task IngestAsync(System.IO.DirectoryInfo drop);
  }
}
