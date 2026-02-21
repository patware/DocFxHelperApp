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

      var sources = new List<SourceSpec>
      {
        new() {
          Id = "a",
          SourceType = SourceType.AdoWiki,
          DisplayName = "A"
        }
      };

      var run = new RunSpec { BuildId = "1", DocFxHelperEngineVersion = "0.0.1", DocFxEngineVersion = "2.78.4" };
      
      var builder = new GraphBuilder();
      var graph = builder.Build(sources, master, run);

      Assert.NotNull(graph);
    }
  }
}
