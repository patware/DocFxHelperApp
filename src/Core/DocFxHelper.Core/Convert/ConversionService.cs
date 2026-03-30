using DocFxHelper.Core.Graph;
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
      [FromKeyedServices(nameof(Specs.DotnetApiSourceSpec))] ISpecConverter dotnetApiConverter,
      [FromKeyedServices(nameof(Specs.RestApiSourceSpec))] ISpecConverter restApiConverter)
    {
      _logger = logger;

      _converters = [];

      _converters.Add(typeof(Specs.AdoWikiSourceSpec), adoWikiConverter);
      _converters.Add(typeof(Specs.DotnetApiSourceSpec), dotnetApiConverter);
      _converters.Add(typeof(Specs.RestApiSourceSpec), restApiConverter);

    }

    public async Task ConvertAsync(Abstractions.Engine.BuildPaths buildPaths, SiteNode siteNode, CancellationToken ct = default!)
    {
      _logger.LogInformation("Checking if {id} needs conversion...", siteNode.Id);

      if (_converters.TryGetValue(siteNode.SourceSpec.GetType(), out var converter))
      {
        _logger.LogInformation("...{id} needs conversion", siteNode.Id);
        await converter.ConvertAsync(buildPaths, siteNode.SourceSpec, siteNode.BuildSpec, ct);
      }
      else
      {
        _logger.LogInformation("...No conversion necessary for {id}", siteNode.Id);
      }
    }
  }
}
