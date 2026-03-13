using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Sources
{
  public interface IIngestion
  {
    /// <summary>
    /// Moves item from the drop folder to the sources folder using the item's ID as the subfolder name.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task IngestAsync(string itemPath, string sourcesPath);

    Task<IReadOnlyList<Specs.SourceSpec>> GetSpecsAsync(string sourcesPath, CancellationToken ct = default);
  }
}
