using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace IGAS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            AddProdSettingsAndPrefixEnvVars(builder).Build().Run();
        }

        internal static IHostBuilder AddProdSettingsAndPrefixEnvVars(IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration(config =>
            {
                // Will be explained in exercise 5
                config.AddJsonFile("prod_appsettings.json", optional: true);

                /// To reduce noise in our API results, locate the EnvironmentVariables 
                /// configuration source in the Host Builder and set a prefix so only 
                /// prefixed environment variables will get injested.
                var envVarConfigSrc = config.Sources.Where(s =>
                   s.GetType() == typeof(EnvironmentVariablesConfigurationSource))
                   .Single();

                ((EnvironmentVariablesConfigurationSource)envVarConfigSrc).Prefix = "IGAS_";
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
