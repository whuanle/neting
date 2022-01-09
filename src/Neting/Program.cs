using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neting;
using Serilog;
using Serilog.Events;
using System;

public class Program
{
    public static int Main(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        NetingConfig.Init(env != null && env.Equals("Development"));
        return StartHost(args);
    }

    private static int StartHost(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
       .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
       .Enrich.FromLogContext()
       .WriteTo.Console()
       .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

#pragma warning disable CS1591
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options =>
                {
                    options.ListenAnyIP(NetingConfig.Instance.Port);
                });
                webBuilder.UseStartup<Startup>();
            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });

#pragma warning disable CS1591
}
