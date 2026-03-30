using DocFxHelper.Core.Building;
using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Tests
{
  public class DocFxBuildEntryFactoryTests
  {
    [Fact]
    public void CreateResource_UsesMediaFoldersForDefaultMediaPattern()
    {
      var node = CreateNode(new AdoWikiSourceSpec
      {
        Id = "guidance",
        DisplayName = "Guidance",
        CloneUrl = "https://example.com/repo.git",
        WikiUrl = "https://example.com/wiki",
        MediaFolders = [".attachments", "media", "screenshots"]
      }, new BuildSpec
      {
        AuthorEmail = "Iam.me@home.net",
        AuthorName = "Iam Me",
        BranchName = "NotMain",
        BuildId = "20250329.0",
        CommitId = "ed21059d6f1d460b0ce3a360d2c6f9050ae82bd2",
        SourceName = "Unit Test"
      });

      var resource = DocFxBuildEntryFactory.CreateResource(node, "**/*.{png,jpg,jpeg,gif,svg,webp,ico}");

      var files = resource["files"]!.AsArray().Select(x => x!.GetValue<string>()).ToArray();

      Assert.Equal(
        [
          ".attachments/**/*.{png,jpg,jpeg,gif,svg,webp,ico}",
          "media/**/*.{png,jpg,jpeg,gif,svg,webp,ico}",
          "screenshots/**/*.{png,jpg,jpeg,gif,svg,webp,ico}"
        ],
        files);
    }

    [Fact]
    public void CreateResource_LeavesDefaultPatternUnchangedWhenMediaFoldersMissing()
    {
      var node = CreateNode(new AdoWikiSourceSpec
      {
        Id = "guidance",
        DisplayName = "Guidance",
        CloneUrl = "https://example.com/repo.git",
        WikiUrl = "https://example.com/wiki"
      }, new BuildSpec
      { 
        AuthorEmail = "Iam.me@home.net",
        AuthorName = "Iam Me",
        BranchName = "NotMain",
        BuildId = "20250329.0",
        CommitId = "ed21059d6f1d460b0ce3a360d2c6f9050ae82bd2",
        SourceName = "Unit Test"
      });

      var resource = DocFxBuildEntryFactory.CreateResource(node, "**/*.{png,jpg,jpeg,gif,svg,webp,ico}");

      var files = resource["files"]!.AsArray().Select(x => x!.GetValue<string>()).ToArray();

      Assert.Equal(["**/*.{png,jpg,jpeg,gif,svg,webp,ico}"], files);
    }

    private static SiteNode CreateNode(SourceSpec sourceSpec, BuildSpec buildSpec)
    {
      return new SiteNode
      {
        Id = sourceSpec.Id,
        DisplayName = sourceSpec.DisplayName,
        Path = "docs",
        ShowInToc = true,
        SourceSpec = sourceSpec,
        BuildSpec = buildSpec,
        DefaultPage = sourceSpec.DefaultPage
      };
    }
  }
}
