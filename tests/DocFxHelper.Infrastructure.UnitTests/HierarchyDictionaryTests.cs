using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;

using DocFxHelper;
using DocFxHelper.Infrastructure;
using Xunit;

namespace DocFxHelper.Infrastructure.UnitTests
{
  /// <summary>
  /// Unit tests for the HierarchyDictionary constructor.
  /// </summary>
  public class HierarchyDictionaryTests
  {
    /// <summary>
    /// Tests that the default constructor initializes Root to null and Items to an empty dictionary.
    /// </summary>
    [Fact]
    public void Constructor_Default_InitializesRootAndItemsCorrectly()
    {
      // Arrange & Act
      var dict = new HierarchyDictionary<string, int>();

      // Assert
      Assert.Null(dict.Root);
      Assert.NotNull(dict.Items);
    }


    /// <summary>
    /// Tests that the parameterized constructor sets Root and Items as provided.
    /// </summary>
    [Fact]
    public void OneItem()
    {
      // Arrange
      var dic = new HierarchyDictionary<string, int>();

      // Act
      dic.Add("a", 1);

      // Assert
      Assert.NotNull(dic.Root);
      Assert.Same(dic.Root, dic.Items["a"]);
      Assert.Same(dic.Root, dic.Root.Root);
      Assert.Equal(1, dic.Root.Item);
      Assert.Equal("/a", dic.Root.Path("/"));

    }

    /// <summary>
    /// Tests that the parameterized constructor allows Root to be null.
    /// </summary>
    [Fact]
    public void TwoItems()
    {
      // Arrange
      var dic = new HierarchyDictionary<string, int>();

      // Act
      dic.Add("a", 1);
      dic.Add("a", "b", 2);


      // Assert
      Assert.NotNull(dic.Items["a"]);
      Assert.NotNull(dic.Items["b"]);
      Assert.NotNull(dic.Root);
      Assert.Same(dic.Items["a"], dic.Root);
      Assert.NotNull(dic.Root.Children);
      Assert.Single(dic.Root.Children);
      Assert.Same(dic.Items["b"].Parent, dic.Items["a"]);
      Assert.Same(dic.Items["b"].Root, dic.Root);
      Assert.Equal("/a/b", dic.Items["b"].Path("/"));

    }

    /// <summary>
    /// Tests that the parameterized constructor allows Root to be null.
    /// </summary>
    [Fact]
    public void ThreeItems()
    {
      // Arrange
      var dic = new HierarchyDictionary<string, int>();

      // Act
      dic.Add("a", 1);
      dic.Add("a", "b", 2);
      dic.Add("b", "c", 3);


      // Assert
      Assert.NotNull(dic.Items["a"]);
      Assert.NotNull(dic.Items["b"]);
      Assert.NotNull(dic.Items["c"]);
      Assert.NotNull(dic.Root);
      Assert.Equal(dic.Root, dic.Items["c"].Root);
      Assert.Equal(dic.Items["b"], dic.Items["c"].Parent);
      Assert.Equal("/a", dic.Items["a"].Path("/"));
      Assert.Equal("/a/b", dic.Items["b"].Path("/"));
      Assert.Equal("/a/b/c", dic.Items["c"].Path("/"));

    }



  }

}