using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IGAS
{
        public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                //Remove the default environment variable source and
                //add it back with a filter so we limit what gets injested.
                config.Sources.RemoveAt(4);
                config.AddEnvironmentVariables("IGAS_");
            })
            .Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
