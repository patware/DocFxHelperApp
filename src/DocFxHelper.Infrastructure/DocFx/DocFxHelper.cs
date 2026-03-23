using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Infrastructure.DocFx
{
  public class DocFxHelper(ILogger<DocFxHelper> logger, IDotnetHelper dotnetHelper) : IDocFxHelper
  {
    private readonly ILogger<DocFxHelper> _logger = logger;
    private readonly IDotnetHelper _dotnetHelper = dotnetHelper;

    private readonly IReadOnlyList<string> _docfx_metadata = ["metadata"];

    private DocFxInstallationMetadata? _installationMetadata;

    public async Task<DocFxInstallationMetadata> GetInstallMetadataAsync(CancellationToken ct = default)
    {
      _logger.LogInformation("Checking if the docfx is installed locally");
      DotnetToolListResult local = await _dotnetHelper.GetDotnetToolPackageAsync("docfx", false, ct);

      DotnetToolListItem? docfx = null;

      var isglobal = false;

      if (local.Data != null && local.Data.Any())
      {
        docfx = local.Data.FirstOrDefault(d => string.Equals(d.PackageId, "docfx", StringComparison.InvariantCultureIgnoreCase));
      }

      if (docfx == null)
      {
        _logger.LogInformation("Checking if the docfx is installed globally");
        isglobal = true;
        DotnetToolListResult global = await _dotnetHelper.GetDotnetToolPackageAsync("docfx", true, ct);

        if (global.Data != null && global.Data.Any())
        {
          docfx = global.Data.FirstOrDefault(d => string.Equals(d.PackageId, "docfx", StringComparison.InvariantCultureIgnoreCase));
        }
      }

      if (docfx == null)
      {
        throw new ApplicationException("Docfx not found locally on globally.");
      }


      _logger.LogInformation("Docfx {version} found {where}", docfx.Version, isglobal ? "Globally" : "Locally");

      _installationMetadata = new DocFxInstallationMetadata
      {
        Command = docfx.Commands.FirstOrDefault()!,
        IsGlobal = isglobal,
        Version = docfx.Version
      };

      return _installationMetadata;

    }

    public async Task<int> RunDocfxMetadataAsync(string workingDirectory, CancellationToken ct = default)
    {
      if (_installationMetadata == null)
      {
        throw new ApplicationException("Dev goof - GetInstallMetadataAsync not ran.");
      }

      _logger.LogInformation("Running docfx to generate the api metadata");
      var result = await _dotnetHelper.RunTool(workingDirectory, _installationMetadata.Command, _docfx_metadata);

      _logger.LogInformation("docfx return code {exitCode}", result.ExitCode);

      return result.ExitCode;
    }
  }
}
