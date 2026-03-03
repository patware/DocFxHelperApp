using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace DocFxHelper.Core.Drops;

public sealed class DropSpec
{
  public const string CurrentSchemaVersion = "1.0";

  [JsonPropertyName("schemaVersion")]
  public string SchemaVersion { get; init; } = CurrentSchemaVersion;

  [JsonPropertyName("dropId")]
  public string DropId { get; init; } = default!;

  [JsonPropertyName("createdUtc")]
  public DateTime CreatedUtc { get; init; }

  [JsonPropertyName("fileCount")]
  public int FileCount { get; init; }


  // ---------- Validation ----------

  public void ValidateStructural()
  {
    if (SchemaVersion != CurrentSchemaVersion)
      throw new InvalidOperationException(
          $"Unsupported schemaVersion '{SchemaVersion}'.");

    if (string.IsNullOrWhiteSpace(DropId))
      throw new InvalidOperationException("DropId is required.");

    if (CreatedUtc == default)
      throw new InvalidOperationException("CreatedUtc must be specified.");

    if (FileCount < 0)
      throw new InvalidOperationException("FileCount cannot be negative.");

  }
}