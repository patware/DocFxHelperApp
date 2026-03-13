using DocFxHelper.Core.Specs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public class AdoWikiConverter(ILogger<AdoWikiConverter> logger) : BaseConverter<Specs.AdoWikiSourceSpec>
  {
    private readonly ILogger<AdoWikiConverter> _logger = logger;

    public override async Task Convert(AdoWikiSourceSpec sourceSpec)
    {
      _logger.LogInformation("Ado Wiki Conversion started");

      await Task.CompletedTask;

      _logger.LogInformation("{id} Converted", sourceSpec.Id);
    }
  }
}
