using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace DocFxHelper.Core.Building
{
  public sealed class DocFxBuildFragment
  {
    public List<JsonObject> Content { get; } = [];
    public List<JsonObject> Resource { get; } = [];
    public JsonObject GlobalMetadata { get; } = [];
    public JsonObject FileMetadata { get; } = [];
    public List<string> Templates { get; } = [];
  }
}
