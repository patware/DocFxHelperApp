using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Specs
{
  /// <summary>
  /// Represents an item in a hierarchical collection, identified by a unique ID and a path relative to a target
  /// directory.
  /// </summary>
  /// <remarks>Use this record to model tree-like structures where each item may contain child items. The 'Id'
  /// property uniquely identifies each item within the hierarchy, while 'TargetRelativePath' specifies its location
  /// relative to a designated target directory. The 'Children' property holds any nested items, allowing for recursive
  /// traversal or manipulation of the hierarchy.</remarks>
  public sealed record NodeItem
  {
    /// <summary>
    /// URI of the node.  Can be relative path, absolute path, or any string that can uniquely identify the node in the context of the source.
    /// </summary>
    /// <remarks>The Node property is required and must be provided during initialization.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("src")]
    public required string Resource { get; init; }

    /// <summary>
    /// Gets the relative path of the source, relative to the parent's root.
    /// </summary>
    /// <remarks>The relative path should be specified in a format compatible with the target environment.
    /// Ensure that the path is correctly formatted to avoid runtime errors.</remarks>
    public string TargetRelativePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of child source items associated with this instance.
    /// </summary>
    /// <remarks>The returned collection is read-only and will be empty if no child items are present.
    /// Modifications to the collection must be performed through the containing object, not directly on this
    /// property.</remarks>
    public IReadOnlyCollection<NodeItem> Children { get; init; }
        = [];
  }

}
