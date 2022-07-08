using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace Tweet.Core
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                          .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                          .Enrich.FromLogContext()
                          .WriteTo.File("logs/tweet-ms-logs-.log", 
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            rollingInterval: RollingInterval.Day)
                          .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/tweet-ms-logs-.json", rollingInterval: RollingInterval.Day)
                          .WriteTo.Console(new ElasticsearchJsonFormatter())
                          .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
                          {
                              AutoRegisterTemplate = true,
                              AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                              IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{DateTime.UtcNow:yyyy-MM}"
                          })
                          .CreateLogger();

            try { 
            
                Log.Information("Tweet Core Microservice Starting Up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Tweet Core Microservice failed to start correctly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
