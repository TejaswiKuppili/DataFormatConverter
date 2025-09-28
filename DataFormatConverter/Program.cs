using DataFormatConverter.Application.Repositories;
using DataFormatConverter.Application.Services;
using DataFormatConverter.Domain.Interfaces;
using DataFormatConverter.Infrastructure.Handlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Configure Function Web Application
builder.ConfigureFunctionsWebApplication();

// Add Application Insights logging
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// ---------------------
// Dependency Injection
// ---------------------

// Register Format Handlers
builder.Services.AddSingleton<IFormatHandler, JsonHandler>();
builder.Services.AddSingleton<IFormatHandler, XmlHandler>();
builder.Services.AddSingleton<IFormatHandler, CsvHandler>();
builder.Services.AddSingleton<IFormatHandler, CanonicalHandler>();


// Register Repository that holds handlers
builder.Services.AddSingleton<FormatHandlerRepository>(sp =>
{
    var handlers = sp.GetServices<IFormatHandler>();
    return new FormatHandlerRepository(handlers);
});

// Register Conversion Service
builder.Services.AddSingleton<ConversionService>();

// Add Logging (optional, already configured by AI Telemetry)
builder.Services.AddLogging();

builder.Build().Run();

















//using DataFormatConverter.API.Functions;
//using DataFormatConverter.Application.Repositories;
//using DataFormatConverter.Application.Services;
//using DataFormatConverter.Domain.Interfaces;
//using DataFormatConverter.Infrastructure.Handlers;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var builder = FunctionsApplication.CreateBuilder(args);

//builder.ConfigureFunctionsWebApplication();

//builder.Services
//    .AddApplicationInsightsTelemetryWorkerService()
//    .ConfigureFunctionsApplicationInsights();

//builder.Build().Run();

//var host = new HostBuilder()
//    .ConfigureFunctionsWorkerDefaults(worker =>
//    {
//        worker.UseMiddleware<ExceptionHandlingMiddleware>();
//    })
//    .ConfigureServices(services =>
//    {
//        // Register handlers
//        services.AddSingleton<IFormatHandler, JsonHandler>();
//        services.AddSingleton<IFormatHandler, XmlHandler>();
//        services.AddSingleton<IFormatHandler, CsvHandler>();

//        // Register repository and service
//        services.AddSingleton<FormatHandlerRepository>(sp =>
//        {
//            var handlers = sp.GetServices<IFormatHandler>();
//            return new FormatHandlerRepository(handlers);
//        });

//        services.AddSingleton<ConversionService>();

//        // Add logging
//        services.AddLogging();
//    })
//    .Build();
