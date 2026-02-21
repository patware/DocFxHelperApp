namespace DocFxHelper.Core.Specs
{
  /// <summary>
  /// Represents the configuration for processing a mustache template, including the template file, output file, scope, and any
  /// optional conditions that determine when the template is applied.
  /// </summary>
  /// <remarks>Use this record to specify the parameters required for generating output from a template. The
  /// Scope property defines the context in which the template is used, such as site-wide or more granular levels. The
  /// Condition property allows for conditional application of the template based on custom logic or criteria.</remarks>
  public sealed record TemplateSpec
  {
    /// <summary>
    /// Path to the mustache template file, relative to the current spec file. The template file should be a valid mustache template that can be processed by DocFxHelper to generate the desired output.
    /// </summary>
    public required string TemplateFile { get; init; }

    /// <summary>
    /// Gets the path of the output file to be generated.  The path should be relative to the target directory and should include the file name and extension. For example, "api/myapi.md" or "conceptual/overview.md". 
    /// The output file will be generated based on the specified template and the data provided to it.
    /// </summary>
    /// <remarks>This property must be initialized with a valid, writable file path before use. The specified
    /// path should be accessible to ensure successful file generation.</remarks>
    public required string OutputFile { get; init; }

    /// <summary>
    /// Gets the condition that determines how the template is processed or rendered.
    /// </summary>
    /// <remarks>If the value is null, no specific condition is applied to the template. Use this property to
    /// control template behavior based on dynamic criteria.</remarks>
    public string[]? RequiredTags { get; init; }
  }

}
