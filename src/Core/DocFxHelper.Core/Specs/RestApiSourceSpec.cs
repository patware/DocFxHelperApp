using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  public class RestApiSourceSpec : SourceSpec
  {
    /// <summary>
    /// Path to the OpenAPI Json file.  example: swagger.json or myapi.json, relative to this file.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("apiJson")]
    public required string ApiJson { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("apiJsonFormat")]
    public required ApiJsonFormats ApiJsonFormat { get; set; }

    public enum ApiJsonFormats
    {
      Swagger2,
      OpenApi3

    }
  }
}
