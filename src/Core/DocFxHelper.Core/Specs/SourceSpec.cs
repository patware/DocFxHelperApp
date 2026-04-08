using System.Text.Json.Serialization;

namespace DocFxHelper.Core.Specs
{
  [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
  [JsonDerivedType(typeof(AdoWikiSourceSpec), typeDiscriminator: nameof(SourceType.AdoWiki))]
  [JsonDerivedType(typeof(DotnetApiSourceSpec), typeDiscriminator: nameof(SourceType.DotnetApi))]
  [JsonDerivedType(typeof(YamlApiSourceSpec), typeDiscriminator: nameof(SourceType.YamlApi))]
  [JsonDerivedType(typeof(RestApiSourceSpec), typeDiscriminator: nameof(SourceType.RestApi))]
  [JsonDerivedType(typeof(ConceptualSourceSpec), typeDiscriminator: nameof(SourceType.Conceptual))]

  ///<summary>
  /// Abstract class that represents the minimum properties every source shares.
  ///</summary>
  public abstract class SourceSpec
  {
    [JsonIgnore]
    public const string FileName = "Source.Spec.Json";
    /// <summary>
    /// Gets the unique identifier for the source.
    /// </summary>
    /// <remarks>The Id property is required and must be provided during initialization. It serves as a
    /// primary key for identifying the sources uniquely within its context.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public required string Id { get; init; }


    ///// <summary>
    ///// Gets the type of source that this instance represents.
    ///// </summary>
    ///// <remarks>The SourceType property is required and must be initialized upon object creation. It defines
    ///// the origin of the data being processed, which can influence how the data is handled or interpreted.</remarks>
    //public required SourceType SourceType { get; init; }


    /// <summary>
    /// Gets the display name of the object, which is used for presentation purposes in navigation and/or menus.
    /// </summary>
    /// <remarks>The display name is required and must be provided during initialization. It is typically used
    /// in user interfaces to represent the object in a user-friendly manner.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("displayName")]
    public required string DisplayName { get; init; }

    [System.Text.Json.Serialization.JsonPropertyName("cloneUrl")]
    public required string CloneUrl { get; init; }

    /// <summary>
    /// Gets the name of the default page to display when no specific page is requested.
    /// </summary>
    /// <remarks>The default page is typically used as a fallback in navigation scenarios. By default, this
    /// property is set to "default.md". Changing this value allows customization of the landing page for documentation
    /// or content navigation.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("defaultPage")]
    public string DefaultPage { get; init; } = $"{Utils.General.Default}.md";

    /// <summary>
    /// Sub-Folder names where images are found
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("mediaFolders")]
    public IReadOnlyList<string>? MediaFolders { get; init; }


    /// <summary>
    /// Gets the collection of tags associated with the item.  Tags can be leveraged in Mustache templates to generate different content based on the presence of specific tags. 
    /// For example, you can have a tag "IsPublic" and use it in your Mustache template to conditionally include or exclude certain sections of the generated documentation.
    /// </summary>
    /// <remarks>The collection is immutable and is initialized to an empty collection if no tags are provided.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("tags")]
    public IReadOnlyCollection<string> Tags { get; init; } = [];

    /// <summary>
    /// Gets the collection of available template specifications.
    /// </summary>
    /// <remarks>The collection is read-only and is initialized to an empty collection if no templates are
    /// specified.</remarks>
    [System.Text.Json.Serialization.JsonPropertyName("templates")]
    public IReadOnlyCollection<TemplateSpec> Templates { get; init; } = [];
  }

}
