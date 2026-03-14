using DocFxHelper.Core.Specs;
using DocFxHelper.Infrastructure;
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
    public async Task LoadAdoWikiSourceSpecAsync_ValidPath_ReturnsSourceSpec()
    {
      // Arrange
      var path = "path/to/valid/source.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("{\r\n  \"type\" : \"AdoWiki\",\r\n\"Id\" : \"SimpleWiki\",\r\n  \"DisplayName\" : \"Simple\",\r\n  \"WikiUrl\" : \"https://dev.azure.com/org/project/_wiki/wikis/wiki-name\"\r\n}");

      var loader = new JsonSpecLoader(fileSystemMock.Object);


      // Act
      var sourceSpec = await loader.LoadSourceSpecAsync(path);

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

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadSourceSpecAsync(path));

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

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadSourceSpecAsync(path));

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
        

      var loader = new JsonSpecLoader(fileSystemMock.Object);


      // Act
      var buildSpec = await loader.LoadBuildSpecAsync(path);

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

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadBuildSpecAsync(path));

    }

    [Fact]
    public async Task LoadBuildSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/build.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadBuildSpecAsync(path));

    }







    [Fact]
    public async Task LoadTemplateSpecAsync_ValidPath_ReturnsTemplateSpec()
    {
      // Arrange
      var path = "path/to/valid/template.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("{\r\n  \"TemplateFile\" : \"me.mustache\",\r\n  \"OutputFile\" : \"Foo/me.md\"\r\n}");

      var loader = new JsonSpecLoader(fileSystemMock.Object);


      // Act
      var templateSpec = await loader.LoadTemplateSpecAsync(path);

      // Assert
      Assert.NotNull(templateSpec);

      Assert.Equal("me.mustache", templateSpec.TemplateFile);
      Assert.Equal("Foo/me.md", templateSpec.OutputFile);

    }


    [Fact]
    public async Task LoadTemplateSpecAsync_ValidPath_ButInvalidJson_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/template.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);

      fileSystemMock.Setup(fs => fs.ReadAllTextAsync(path, It.IsAny<CancellationToken>()))
        .ReturnsAsync("null");

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadTemplateSpecAsync(path));

    }

    [Fact]
    public async Task LoadTemplateSpecAsync_InvalidPath_ThrowsException()
    {
      // Arrange
      var path = "path/to/valid/template.spec.json";

      var fileSystemMock = new Mock<IFileSystem>();

      fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(false);

      var loader = new JsonSpecLoader(fileSystemMock.Object);

      // Act

      await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadTemplateSpecAsync(path));

    }


  }
}
