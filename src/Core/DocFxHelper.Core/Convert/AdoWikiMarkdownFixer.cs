using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DocFxHelper.Core.Convert
{
  internal static partial class AdoWikiMarkdownFixer
  {
    [GeneratedRegex(
    @"(?<include>\[!include)?\[(?<display>(?:[^\[\]]|(?<Open>\[)|(?<Content-Open>\]))+(?(Open)(?!)))\]\((?<link>(?:[^\(\)]|(?<Open>\()|(?<Content-Open>\)))+(?(Open)(?!)))\)",
    RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MarkdownLinkRegex();

    [GeneratedRegex(@"^```[\t ]*\w*[\t ]*$", RegexOptions.Compiled)]
    private static partial Regex FenceRegex();

    public static string FixAdoWikiEscapes(string content)
    {
      if (string.IsNullOrEmpty(content))
      {
        return content;
      }

      var lines = SplitLines(content);
      var output = new StringBuilder(content.Length);
      var inFence = false;

      foreach (var line in lines)
      {
        if (FenceRegex().IsMatch(line.Text))
        {
          inFence = !inFence;
          output.Append(line.Raw);
          continue;
        }

        if (inFence)
        {
          output.Append(line.Raw);
          continue;
        }

        var updated = MarkdownLinkRegex().Replace(line.Text, static match =>
        {
          if (match.Groups["include"].Success)
          {
            return match.Value;
          }

          var display = match.Groups["display"].Value;
          var link = match.Groups["link"].Value
            .Replace(@"\(", "(")
            .Replace(@"\)", ")");

          return $"[{display}]({link})";
        });

        output.Append(updated);
        output.Append(line.NewLine);
      }

      return output.ToString();
    }

    private static IReadOnlyList<(string Text, string Raw, string NewLine)> SplitLines(string content)
    {
      var lines = new List<(string Text, string Raw, string NewLine)>();
      using var reader = new StringReader(content);

      while (reader.ReadLine() is { } line)
      {
        var nextIndex = line.Length;
        lines.Add((line, line, Environment.NewLine));
      }

      if (content.EndsWith("\r\n", StringComparison.Ordinal) ||
          content.EndsWith("\n", StringComparison.Ordinal))
      {
        return lines;
      }

      if (lines.Count > 0)
      {
        var last = lines[^1];
        lines[^1] = (last.Text, last.Raw, string.Empty);
      }

      return lines;
    }
  }
}
