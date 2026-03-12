using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;

namespace DocFxHelper.Core.Tests
{
  public class GraphBuilderTests
  {
    [Fact]
    public void Build_SingleSource_GraphHasOneNode()
    {

      var master = new MasterSpec
      {
        Root = new NodeItem
        {
          Resource = "..\a"
        }
      };

      var sources = new List<AdoWikiSourceSpec>
      {
        new() {
          Id = "a",
          DisplayName = "A",
          WikiUrl = "https://dev.azure.com/organization/project/_wiki/wikis/wiki-name"
        }
      };

      //var run = new RunMeta
      //{
      //  Id = 1,
      //  BuildId = "20250101.1",
      //  DocFxEngineVersion = "1.0.0",
      //  DocFxHelperEngineVersion = "2.7"
      //};

      var builder = new GraphBuilder();
      var graph = builder.Build(sources, master);

      Assert.NotNull(graph);
    }
  }
}
