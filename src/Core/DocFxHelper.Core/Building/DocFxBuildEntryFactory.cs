using DocFxHelper.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace DocFxHelper.Core.Building
{
  internal static class DocFxBuildEntryFactory
  {
    private const string DefaultMediaPattern = "**/*.{png,jpg,jpeg,gif,svg,webp,ico}";

    public static JsonObject CreateBaseContent(SiteNode node)
    {
      return new JsonObject()
      {
        ["exclude"] = new JsonArray("_site/**"),
        ["src"] = $"{node.Id}/",
        ["dest"] = node.Dest
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
      resource["dest"] = ".";
      resource["files"] = new JsonArray(GetResourceFilePatterns(node, files).Select(x => (JsonNode)x).ToArray());
      return resource;
    }

    private static IReadOnlyList<string> GetResourceFilePatterns(SiteNode node, IReadOnlyList<string> files)
    {
      var mediaFolders = node.SourceSpec.MediaFolders?
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(x => x.Trim().Trim('/', '\\'))
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

      if (mediaFolders is null || mediaFolders.Length == 0)
      {
        return files.ToArray();
      }

      var patterns = new List<string>();

      foreach (var file in files)
      {
        if (string.Equals(file, DefaultMediaPattern, StringComparison.OrdinalIgnoreCase))
        {
          patterns.AddRange(mediaFolders.Select(folder => $"{folder}/{DefaultMediaPattern}"));
          continue;
        }

        patterns.Add(file);
      }

      return patterns;
    }
  }
}
