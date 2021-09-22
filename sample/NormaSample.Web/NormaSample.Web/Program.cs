using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace NormaSample.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.MinimumLevel.Verbose()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithRootName("ExceptionDetails"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
