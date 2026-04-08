using DocFxHelper.Core.Convert;
using DocFxHelper.Core.Utils;
using DocFxHelper.Infrastructure.DocFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration
  .SetBasePath(AppContext.BaseDirectory)
  .AddJsonFile("appsettings.json", optional:false);

var config = new DocFxHelper.ConsoleApp.Configuration();
builder.Configuration.GetSection(DocFxHelper.ConsoleApp.Configuration.SectionName).Bind(config);

SetWorkingDirectory(config.WorkingDirectory);

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Replace the default logging providers with Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddTransient<DocFxHelper.Infrastructure.ITimeService, DocFxHelper.Infrastructure.SystemTimeService>();
builder.Services.AddTransient<DocFxHelper.Infrastructure.IFileSystem, DocFxHelper.Infrastructure.FileSystem>();

builder.Services.AddTransient<DocFxHelper.Core.Specs.ISpecLoader, DocFxHelper.Core.Specs.JsonSpecLoader>();
builder.Services.AddTransient<DocFxHelper.Core.Drops.IScout, DocFxHelper.Core.Drops.Scout>();
builder.Services.AddTransient<DocFxHelper.Core.Sources.IIngestion, DocFxHelper.Core.Sources.Ingestion>();
builder.Services.AddTransient<IVerification, Verification>();
builder.Services.AddTransient<DocFxHelper.Core.Convert.IConversion, DocFxHelper.Core.Convert.ConversionService>();
builder.Services.AddTransient<DocFxHelper.Core.Engine.IProcessor, DocFxHelper.Core.Engine.NaiveSequentialProcessor>();
builder.Services.AddTransient<DocFxHelper.Core.Utils.ITocHelper, DocFxHelper.Core.Utils.TocHelper>();
builder.Services.AddTransient<DocFxHelper.Core.Utils.IGeneral, DocFxHelper.Core.Utils.General>();
builder.Services.AddTransient<DocFxHelper.Core.Graph.IGraphBuilder, DocFxHelper.Core.Graph.GraphBuilder>();
builder.Services.AddTransient<DocFxHelper.Core.Building.IAssembly, DocFxHelper.Core.Building.Assembly>();
builder.Services.AddTransient<DocFxHelper.Core.Building.IAssemblyHelper, DocFxHelper.Core.Building.AssemblyHelper>();
builder.Services.AddTransient<DocFxHelper.Infrastructure.IDotnetHelper, DocFxHelper.Infrastructure.DotnetHelper>();

builder.Services.AddSingleton<DocFxHelper.Core.Building.IDocFxBuildSourceContributor, DocFxHelper.Core.Building.RestApiDocFxBuildSourceContributor>();
builder.Services.AddSingleton<DocFxHelper.Core.Building.IDocFxBuildSourceContributor, DocFxHelper.Core.Building.DefaultDocFxBuildSourceContributor>();

builder.Services.AddKeyedTransient<DocFxHelper.Core.Convert.ISpecConverter, DocFxHelper.Core.Convert.AdoWikiConverter>(nameof(DocFxHelper.Core.Specs.AdoWikiSourceSpec));
builder.Services.AddKeyedTransient<DocFxHelper.Core.Convert.ISpecConverter, DocFxHelper.Core.Convert.DotnetApiConverter>(nameof(DocFxHelper.Core.Specs.DotnetApiSourceSpec));
builder.Services.AddKeyedTransient<DocFxHelper.Core.Convert.ISpecConverter, DocFxHelper.Core.Convert.RestApiConverter>(nameof(DocFxHelper.Core.Specs.RestApiSourceSpec));


builder.Services.AddSingleton<IDocFxHelper, DocFxHelper.Infrastructure.DocFx.DocFxHelper>();

builder.Services.AddTransient<DocFxHelper.ConsoleApp.App>();

using IHost host = builder.Build();

Log.Information("DocFxHelper {version} started", Assembly.GetExecutingAssembly().GetName().Version);
Log.Information("  Args: [{args}]", args);
Log.Information("  Working Directory: [{workingDirectory}]", Environment.CurrentDirectory);

var app = host.Services.GetRequiredService<DocFxHelper.ConsoleApp.App>();

await app.RunAsync();

Log.Information("Application is shutting down...");

await Log.CloseAndFlushAsync();


static void SetWorkingDirectory(string configuredPath)
{
  if (string.IsNullOrWhiteSpace(configuredPath) || configuredPath == ".")
    return;

  // Resolve relative paths to absolute paths
  var fullPath = Path.GetFullPath(configuredPath);

  if (!Directory.Exists(fullPath))
  {
    throw new DirectoryNotFoundException(
        $"Configured working directory does not exist: {fullPath}");
  }

  Directory.SetCurrentDirectory(fullPath);
}