using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Specs;
using DocFxHelper.Core.Utils;
using DocFxHelper.Infrastructure.DocFx;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public class DotnetApiConverter(
    ILogger<DotnetApiConverter> logger, 
    Infrastructure.IFileSystem fileSystem,
    IGeneral general,
    IDocFxHelper docfxHelper) : BaseConverter<Specs.DotnetApiSourceSpec>
  {
    private readonly ILogger<DotnetApiConverter> _logger = logger;
    private readonly Infrastructure.IFileSystem _fileSystem = fileSystem;
    private readonly IGeneral _general = general;
    private readonly IDocFxHelper _docfxHelper = docfxHelper;

    public override async Task<int> Convert(BuildPaths buildPaths, DotnetApiSourceSpec sourceSpec, BuildSpec buildSpec, CancellationToken ct = default)
    {
      _logger.LogInformation("---------------");
      _logger.LogInformation("Dotnet Api [{id}] Conversion started", sourceSpec.Id);

      var sourceFolder = System.IO.Path.Combine(buildPaths.Sources, sourceSpec.Id);
      var convertedFolder = System.IO.Path.Combine(buildPaths.Converted, sourceSpec.Id);

      _logger.LogInformation("Step 0 - Clean up target");
      if (_fileSystem.DirectoryExists(convertedFolder))
      {
        _logger.LogInformation("Deleting converted folder [{converted}] - clean folder", Path.GetRelativePath(buildPaths.WorkingDirectory, convertedFolder));
        _fileSystem.DeleteDirectory(convertedFolder);
      }

      _logger.LogInformation("Step 1 - Verify docfx.json");

      var docfxJson = Path.Combine(sourceFolder, "docfx.json");

      if (File.Exists(docfxJson))
      {
        _logger.LogInformation("Using provided docfx.json");
      }
      else
      {
        _logger.LogInformation("Generating generic docfx.json");
        var docfx_metadata = DocFxHelper.Core.Properties.Resources.docfx_metadata;

        if (docfx_metadata == null)
        {
          throw new ApplicationException("Dev goof - docfx_metadata not embedded in assembly");
        }

        docfx_metadata = docfx_metadata.Replace("api", convertedFolder.Replace("\\", "\\\\"));

        await _fileSystem.WriteAllTextAsync(docfxJson, docfx_metadata);
      }

      _logger.LogInformation("Calling docfx to generate the api metadata");
      var result = await _docfxHelper.RunDocfxMetadataAsync(sourceFolder, ct);

      _logger.LogInformation("Dotnet Api [{id}] Converted", sourceSpec.Id);
      return result;

    }
  }
}
