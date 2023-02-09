using IdentityServer;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
    builder.Host.UseSerilog();

    Log.Logger = logger;

    Log.Information("Starting up");

    // Log all environment variables
    Log.Information("Environment Variables:");
    foreach (var c in builder.Configuration.AsEnumerable())
    {
        Log.Information(c.Key + " = " + c.Value);
    }

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name != "StopTheHostException")
    {
        Log.Fatal(ex, "Unhandled exception");
    }
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}