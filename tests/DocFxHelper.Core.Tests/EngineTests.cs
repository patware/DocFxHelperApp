using DocFxHelper.Core.Engine;
using Moq;

namespace DocFxHelper.Core.Tests;

public class EngineTests
{
  [Fact]
  public async Task BuildAsync_ModeFull_ReturnsSuccess()
  {
    var timeServiceMock = new Mock<Abstractions.ITimeService>();

    timeServiceMock.SetupGet(m => m.UtcNow).Returns(new DateTimeOffset(2026, 02, 01, 12, 00, 00, TimeSpan.Zero));

    var engine = new DocFxHelperEngine(timeServiceMock.Object);

    var result = await engine.BuildAsync(new Abstractions.Engine.BuildRequest 
      { 
        Sources = [],
        Mode = Abstractions.Engine.BuildMode.Full
      }, 
      TestContext.Current.CancellationToken);
  }

  [Fact]
  public async Task BuildAsync_ModeIncremental_ReturnsSuccess()
  {
    var timeServiceMock = new Mock<Abstractions.ITimeService>();

    timeServiceMock.SetupGet(m => m.UtcNow).Returns(new DateTimeOffset(2026,02,01, 12, 00, 00, TimeSpan.Zero));

    var engine = new DocFxHelperEngine(timeServiceMock.Object);

    var result = await engine.BuildAsync(
      new Abstractions.Engine.BuildRequest
      {
        Sources = [],
        Mode = Abstractions.Engine.BuildMode.Incremental
      },
      TestContext.Current.CancellationToken);
  }
}
