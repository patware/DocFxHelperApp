using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed class ConceptualSourceSpec : SourceSpec
  {
    /// <summary>
    /// Docs folder in repo.  Default: /docs
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("docsFolder")]
    public string DocsFolder { get; init; } = "/docs";

  }
}
