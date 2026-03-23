using DocFxHelper.Core.Specs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Convert
{
  public class ConversionService : IConversion
  {
    private readonly ILogger<ConversionService> _logger;
    private readonly Dictionary<Type, ISpecConverter> _converters;
        
    public ConversionService(ILogger<ConversionService> logger, 
      [FromKeyedServices(nameof(Specs.AdoWikiSourceSpec))] ISpecConverter adoWikiConverter,
      [FromKeyedServices(nameof(Specs.DotnetApiSourceSpec))] ISpecConverter dotnetApiConverter)
    {
      _logger = logger;

      _converters = [];

      _converters.Add(typeof(Specs.AdoWikiSourceSpec), adoWikiConverter);
      _converters.Add(typeof(Specs.DotnetApiSourceSpec), dotnetApiConverter);

    }

    public async Task ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, SourceSpec sourceSpec, CancellationToken ct = default!)
    {
      _logger.LogInformation("Checking if {id} needs conversion...", sourceSpec.Id);

      if (_converters.TryGetValue(sourceSpec.GetType(), out var converter))
      {
        _logger.LogInformation("...{id} needs conversion", sourceSpec.Id);
        await converter.ConvertAsync(buildPaths, sourceSpec, ct);
      }
      else
      {
        _logger.LogInformation("...No conversion necessary for {id}", sourceSpec.Id);
      }
    }
  }
}
