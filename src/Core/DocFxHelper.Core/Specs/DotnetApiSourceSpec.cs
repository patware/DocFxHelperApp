using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public class DotnetApiSourceSpec : SourceSpec
  {
    /// <summary>
    /// Gets or sets the path to the DocFX JSON configuration file used for documentation generation.
    /// </summary>
    /// <remarks>Specify the file system path to the DocFX JSON file, relative to the current master spec file.  
    /// If no docfx.json is provided, a default empty one will be provided to you.
    /// Provide your own docfx.json if you want to customize the docfx metadata
    /// </remarks>
    [System.Text.Json.Serialization.JsonPropertyName("docFxJsonPath")]
    public string? DocFxJsonPath { get; set; }
  }
}
