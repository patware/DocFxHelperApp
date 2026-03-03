using DocFxHelper.Core.Specs;

namespace DocFxHelper.Core
{
  /// <summary>
  /// Represents the details for a documentation generation run, including build identification, timestamps, and
  /// engine version information.
  /// </summary>
  /// <remarks>Use this record to encapsulate metadata about a specific execution of the documentation
  /// generation process. The information provided by this type can be used for auditing, tracking, or associating
  /// generated documentation with a particular build and engine version. All properties are immutable after
  /// initialization.</remarks>
  public sealed record Run
  {
    /// <summary>
    /// Unique identifier
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Gets the unique identifier for the build associated with the current instance in the form of yyyyMMdd.rev
    /// </summary>
    /// <remarks>It is typically used to track specific builds in a continuous integration or deployment pipeline.
    /// The BuildId is generated at runtime by DocFxHelper using year month day and a revision number
    /// </remarks>
    public required string BuildId { get; init; }

    /// <summary>
    /// Gets the date and time, in Coordinated Universal Time (UTC), when the Run was started.
    /// </summary>
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the version of the DocFxHelper engine used for documentation generation.
    /// </summary>
    public required string DocFxHelperEngineVersion { get; init; }

    /// <summary>
    /// Gets the version of the DocFx helper engine used for documentation generation.
    /// </summary>
    public required string DocFxEngineVersion { get; init; }

    public IReadOnlyCollection<SourceSpec> Templates { get; init; } = [];





  }
}
