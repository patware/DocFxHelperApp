namespace DocFxHelper.Core.Specs
{
  /// <summary>
  /// Represents the master specification for documentation generation, including the root source item, the path to the
  /// DocFX configuration file, and the collection of available template specifications.
  /// </summary>
  /// <remarks>Use this type to encapsulate all necessary information for configuring and initiating a
  /// documentation generation process. The root source item defines the starting point for documentation, while the
  /// optional DocFX JSON path allows customization of the build process. The collection of template specifications
  /// provides access to available templates for generating documentation instances.</remarks>
  public sealed record MasterSpec
  {

    /// <summary>
    /// Gets or sets the root source item for the current context.
    /// </summary>
    /// <remarks>This property must be initialized before use. It represents the primary source item that
    /// serves as the starting point for operations within the context.</remarks>
    public required NodeItem Root { get; set; }

    /// <summary>
    /// Gets or sets the path to the DocFX JSON configuration file used for documentation generation.
    /// </summary>
    /// <remarks>Specify the file system path to the DocFX JSON file, relative to the current master spec file.  
    /// If no docfx.json is provided, a default empty one will be provided to you.
    /// Provide your own docfx.json if you want to customize the docfx build, for example, by providing custom templates or global metadata.
    /// The final docfx.json used for the docfx build will be a merged result of the provided docfx.json and a default empty docfx.json, with the provided one taking precedence in case of conflicts.
    /// </remarks>
    public string? DocFxJsonPath { get; set; }

    /// <summary>
    /// Gets the collection of available template specifications.
    /// </summary>
    /// <remarks>This property provides read-only access to the templates, which can be used to create
    /// instances based on the specified template specifications.</remarks>
    public IReadOnlyCollection<TemplateSpec> Templates { get; init; } = [];
  }

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
