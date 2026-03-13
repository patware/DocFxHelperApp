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
builder.Services.AddTransient<DocFxHelper.Core.Convert.IConversion, DocFxHelper.Core.Convert.ConversionService>();
builder.Services.AddTransient<DocFxHelper.Core.Convert.ISpecConverter, DocFxHelper.Core.Convert.AdoWikiConverter>();
builder.Services.AddTransient<DocFxHelper.Core.Engine.IProcessor, DocFxHelper.Core.Engine.NaiveSequentialProcessor>();
builder.Services.AddTransient<DocFxHelper.Core.Utils.IGeneral, DocFxHelper.Core.Utils.General>();

builder.Services.AddTransient<DocFxHelper.ConsoleApp.App>();

using IHost host = builder.Build();

Log.Information("DocFxHelper {version} started", Assembly.GetExecutingAssembly().GetName().Version);
Log.Information("  Args: [{args}]", args);
Log.Information("  Working Directory: [{workingDirectory}]", Environment.CurrentDirectory);

var app = host.Services.GetRequiredService<DocFxHelper.ConsoleApp.App>();

await app.RunAsync();

Log.Information("Application is shutting down...");

await Log.CloseAndFlushAsync();