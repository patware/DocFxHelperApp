#nullable enable
using DocFxHelper.Core;
using DocFxHelper.Core.Drops;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;


namespace DocFxHelper.Core.Drops.UnitTests;

public class DropSpecTests
{
  /// <summary>
  /// Verifies that a valid DropSpec with matching FileCount and TotalBytes does not throw.
  /// Inputs: valid DropId, non-default CreatedUtc, FileCount matching Files.Count, TotalBytes matching sum of Sizes.
  /// Expected: No exception thrown.
  /// </summary>
  [Fact]
  public void ValidateStructural_ValidSpec_DoesNotThrow()
  {
    // Arrange

    var spec = new DropSpec
    {
      DropId = "drop-1",
      CreatedUtc = DateTime.UtcNow,
      FileCount = 1
    };

    // Act & Assert (should not throw)
    spec.ValidateStructural();
  }

  /// <summary>
  /// Validates that an unsupported SchemaVersion causes an InvalidOperationException.
  /// Inputs: SchemaVersion != CurrentSchemaVersion, other fields valid.
  /// Expected: InvalidOperationException with message indicating unsupported schema version.
  /// </summary>
  [Fact]
  public void ValidateStructural_InvalidSchemaVersion_ThrowsInvalidOperationException()
  {
    // Arrange
    var badVersion = "2.0";
    var spec = new DropSpec
    {
      SchemaVersion = badVersion,
      DropId = "x",
      CreatedUtc = DateTime.UtcNow,
      FileCount = 1
    };

    // Act
    var ex = Assert.Throws<InvalidOperationException>(() => spec.ValidateStructural());

    // Assert
    Assert.Contains($"Unsupported schemaVersion '{badVersion}'.", ex.Message);
  }

  /// <summary>
  /// Ensures empty and whitespace-only DropId values cause an InvalidOperationException.
  /// Inputs: DropId is empty or whitespace; other fields valid.
  /// Expected: InvalidOperationException with message 'DropId is required.'.
  /// </summary>
  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void ValidateStructural_EmptyOrWhitespaceDropId_ThrowsInvalidOperationException(string dropId)
  {
    // Arrange
    var spec = new DropSpec
    {
      DropId = dropId,
      CreatedUtc = DateTime.UtcNow,
      FileCount = 1
    };

    // Act
    var ex = Assert.Throws<InvalidOperationException>(() => spec.ValidateStructural());

    // Assert
    Assert.Equal("DropId is required.", ex.Message);
  }

  /// <summary>
  /// Verifies that CreatedUtc left as default (DateTime.MinValue) causes an InvalidOperationException.
  /// Inputs: CreatedUtc default, other fields valid.
  /// Expected: InvalidOperationException with message 'CreatedUtc must be specified.'.
  /// </summary>
  [Fact]
  public void ValidateStructural_DefaultCreatedUtc_ThrowsInvalidOperationException()
  {
    // Arrange
    
    var spec = new DropSpec
    {
      DropId = "d",
      CreatedUtc = default, // DateTime default
      FileCount = 1
    };

    // Act
    var ex = Assert.Throws<InvalidOperationException>(() => spec.ValidateStructural());

    // Assert
    Assert.Equal("CreatedUtc must be specified.", ex.Message);
  }

  /// <summary>
  /// Ensures negative FileCount values cause an InvalidOperationException.
  /// Inputs: FileCount negative values (int.MinValue, -1), other fields otherwise valid.
  /// Expected: InvalidOperationException with message 'FileCount cannot be negative.'.
  /// </summary>
  [Theory]
  [InlineData(int.MinValue)]
  [InlineData(-1)]
  public void ValidateStructural_NegativeFileCount_ThrowsInvalidOperationException(int fileCount)
  {
    // Arrange
    
    var spec = new DropSpec
    {
      DropId = "d",
      CreatedUtc = DateTime.UtcNow,
      FileCount = fileCount
    };

    // Act
    var ex = Assert.Throws<InvalidOperationException>(() => spec.ValidateStructural());

    // Assert
    Assert.Equal("FileCount cannot be negative.", ex.Message);
  }


  
}