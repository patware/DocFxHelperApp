using DocFxHelper.Infrastructure.SpecLoading;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure.Tests.SpecLoading
{
  public class JsonSpecLoaderTests
  {
    [Fact]
    public async Task LoadMasterSpecAsync_ValidPath_ReturnsMasterSpec()
    {
      // Arrange
      var path = "path/to/valid/master.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("{\r\n  \"Root\": {\r\n    \"src\": \"../SimpleDocSiteWiki\"\r\n  }\r\n}\r\n");

      var loader = new JsonSpecLoader(fileSystemMock.Object);
           

      // Act
      var masterSpec = await loader.LoadMasterSpecAsync(path);

      // Assert
      Assert.NotNull(masterSpec);

      Assert.Equal("../SimpleDocSiteWiki", masterSpec.Root.Resource);
    }


    [Fact]
    public async Task LoadMasterSpecAsync_ValidPath_ButInvalidJson_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/master.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("null");

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadMasterSpecAsync(path));
      
    }

    [Fact]
    public async Task LoadMasterSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/master.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadMasterSpecAsync(path));

    }



    [Fact]
    public async Task LoadSourceSpecAsync_ValidPath_ReturnsSourceSpec()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("{\r\n  \"Id\" : \"SimpleWiki\",\r\n  \"SourceType\" : \"AdoWiki\",\r\n  \"DisplayName\" : \"Simple\"\r\n}");

      var loader = new JsonSpecLoader(fileSystemMock.Object);


      // Act
      var sourceSpec = await loader.LoadSourceSpecAsync(path);

      // Assert
      Assert.NotNull(sourceSpec);

      Assert.Equal("SimpleWiki", sourceSpec.Id);
      Assert.Equal(Core.Specs.SourceType.AdoWiki, sourceSpec.SourceType);
      Assert.Equal("Simple", sourceSpec.DisplayName);
    }


    [Fact]
    public async Task LoadSourceSpecAsync_ValidPath_ButInvalidJson_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("null");

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadMasterSpecAsync(path));

    }

    [Fact]
    public async Task LoadSourceSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadMasterSpecAsync(path));

    }



  }
}
