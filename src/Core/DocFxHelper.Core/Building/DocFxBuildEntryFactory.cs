using DocFxHelper.Core.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace DocFxHelper.Core.Building
{
  internal static class DocFxBuildEntryFactory
  {
    public static JsonObject CreateBaseContent(SiteNode node)
    {
      return new JsonObject()
      {
        ["exclude"] = new JsonArray("_site/**"),
        ["src"] = $"{node.Id}/",
        ["dest"] = node.Path ?? string.Empty
      };
    }

    public static JsonObject CreateMarkdownContent(SiteNode node)
    {
      var content = CreateBaseContent(node);
      content["files"] = new JsonArray("**/*.{md,yml}");
      return content;
    }

    public static JsonObject CreateResource(SiteNode node, params string[] files)
    {
      var resource = CreateBaseContent(node);
      resource["files"] = new JsonArray(files.Select(x => (JsonNode)x).ToArray());
      return resource;
    }
  }
}
