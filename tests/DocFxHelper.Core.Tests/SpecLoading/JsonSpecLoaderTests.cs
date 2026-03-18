using DocFxHelper.Core.Specs;
using DocFxHelper.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Tests.SpecLoading
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
        .ReturnsAsync("{\r\n  \"Root\": {\r\n    \"id\": \"SimpleWiki\"\r\n  }\r\n}\r\n");

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);
           

      // Act
      var masterSpec = await loader.LoadMasterSpecAsync(path, TestContext.Current.CancellationToken);

      // Assert
      Assert.NotNull(masterSpec);

      Assert.NotNull(masterSpec.Root);

      Assert.Equal("SimpleWiki", masterSpec.Root!.ResourceId);
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

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadMasterSpecAsync(path, TestContext.Current.CancellationToken));
      
    }

    [Fact]
    public async Task LoadMasterSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/master.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadMasterSpecAsync(path, TestContext.Current.CancellationToken));

    }





    [Fact]
    public async Task LoadAdoWikiSourceSpecAsync_ValidPath_ReturnsSourceSpec()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("{\r\n  \"type\" : \"AdoWiki\",\r\n\"Id\" : \"SimpleWiki\",\r\n  \"DisplayName\" : \"Simple\",\r\n  \"WikiUrl\" : \"https://dev.azure.com/org/project/_wiki/wikis/wiki-name\"\r\n}");

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);


      // Act
      var sourceSpec = await loader.LoadSourceSpecAsync(path,TestContext.Current.CancellationToken);

      // Assert
      Assert.NotNull(sourceSpec);

      Assert.Equal("SimpleWiki", sourceSpec.Id);      
      Assert.Equal("Simple", sourceSpec.DisplayName);      
      Assert.IsType<AdoWikiSourceSpec>(sourceSpec);

      var adoWikiSourceSpec = sourceSpec as AdoWikiSourceSpec;
      Assert.Equal("https://dev.azure.com/org/project/_wiki/wikis/wiki-name", adoWikiSourceSpec?.WikiUrl);

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

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadSourceSpecAsync(path, TestContext.Current.CancellationToken));

    }

    [Fact]
    public async Task LoadSourceSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadSourceSpecAsync(path, TestContext.Current.CancellationToken));

    }




    [Fact]
    public async Task LoadBuildSpecAsync_ValidPath_ReturnsBuildSpec()
    {
      // Arrange
      var path = "path/to/valid/build.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("""        
        {
          "BuildId" : "123456",
          "SourceName" : "PipelineX",
          "BranchName" : "Main",
          "CommitId" : "1234",
          "AuthorName" : "Author Name",
          "AuthorEmail" : "Author@email.com"
        }        
        """);
        

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);


      // Act
      var buildSpec = await loader.LoadBuildSpecAsync(path, TestContext.Current.CancellationToken);

      // Assert
      Assert.NotNull(buildSpec);

      Assert.Equal("123456", buildSpec.BuildId);
      Assert.Equal("PipelineX", buildSpec.SourceName);
      Assert.Equal("Main", buildSpec.BranchName);
      Assert.Equal("1234", buildSpec.CommitId);
      Assert.Equal("Author Name", buildSpec.AuthorName);
      Assert.Equal("Author@email.com", buildSpec.AuthorEmail);
      
    }


    [Fact]
    public async Task LoadBuildSpecAsync_ValidPath_ButInvalidJson_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/build.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("null");

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadBuildSpecAsync(path, TestContext.Current.CancellationToken));

    }

    [Fact]
    public async Task LoadBuildSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/build.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(new NullLogger<JsonSpecLoader>(), fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadBuildSpecAsync(path, TestContext.Current.CancellationToken));

    }

  }
}
