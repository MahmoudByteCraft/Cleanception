using Cleanception.Application;
using Cleanception.Application.Common.Validation;
using Cleanception.Host.Configurations;
using Cleanception.Host.Controllers;
using Cleanception.Infrastructure;
using Cleanception.Infrastructure.Common;
using Serilog;
using Hangfire;

[assembly: ApiConventionType(typeof(UPCApiConventions))]

StaticLogger.EnsureInitialized();
Log.Information(@"
   ____ _                                 _   _             
  / ___| | ___  __ _ _ __   ___ ___ _ __ | |_(_) ___  _ __  
 | |   | |/ _ \/ _` | '_ \ / __/ _ \ '_ \| __| |/ _ \| '_ \ 
 | |___| |  __/ (_| | | | | (_|  __/ |_) | |_| | (_) | | | |
  \____|_|\___|\__,_|_| |_|\___\___| .__/ \__|_|\___/|_| |_|
                                   |_|                      
");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
        .ReadFrom.Configuration(builder.Configuration);
    });

    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    builder.Services.AddApplication();

    builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

    // builder.WebHost.ConfigureKestrel(options =>
    // {
    //     options.ListenAnyIP(5001); // to listen for incoming http connection on port 5001
    //     options.ListenAnyIP(7001, configure => configure.UseHttps()); // to listen for incoming https connection on port 7001
    // });

    var app = builder.Build();

    // app.UseCors("MyPolicy");

    await app.Services.InitializeDatabasesAsync();

    app.UseInfrastructure(builder.Configuration);

    app.MapEndpoints();

    AddBackgroundJobs();

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}

void AddBackgroundJobs()
{

}