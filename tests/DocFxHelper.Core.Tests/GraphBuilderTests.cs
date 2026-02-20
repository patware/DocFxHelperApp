using DocFxHelper.Core.Graph;
using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core.Tests
{
  public class GraphBuilderTests
  {
    [Fact]
    public void Build_SingleSource_GraphHasOneNode()
    {
      var sources = new[] {
        new SourceSpec { Id = "a", DisplayName = "Source A", TargetPath = "/a", SourceType = "Wiki"}
      };

      var master = new MasterSpec { RootPath = "/" };

      var run = new RunSpec { BuildId = "1", EngineVersion = "0.0.1", Timestamp = DateTimeOffset.UtcNow };

      var builder = new GraphBuilder();
      var graph = builder.Build(sources, master, run);

      Assert.Equal("id", graph.Root.Id);
    }
  }
}
