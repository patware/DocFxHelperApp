using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public sealed class AdoWikiSourceSpec : SourceSpec
  {
    [System.Text.Json.Serialization.JsonPropertyName("wikiUrl")]
    public required string WikiUrl { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("topMenuReferenced")]

    public bool TopMenuReferenced { get; init; } = false;

    /// <summary>
    ///  List of Pages to promote to their sub folder
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("promote")]
    public List<string>? Promote { get; init; }
  }
}
