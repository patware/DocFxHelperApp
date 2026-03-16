using DocFxHelper.Abstractions.Engine;
using DocFxHelper.Core.Graph;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocFxHelper.Core.Building
{
  public class Assembly(ILogger<Assembly> logger) : IAssembly
  {

    private readonly ILogger<Assembly> _logger = logger;
    
    public async Task Assemble(BuildPaths buildPaths)
    {
      _logger.LogInformation("Assembly started");
            
      _logger.LogInformation("Assembly completed");
      return;

    }
  }
}
