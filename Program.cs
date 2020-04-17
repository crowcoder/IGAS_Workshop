using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            AddPrefixToEnvVarConfigSrc(builder).Build().Run();
        }

        /// Locate the EnvironmentVariables configuration source in the Host Builder and
        /// set a prefix so only prefixed environment variables will get injested.
        internal static IHostBuilder AddPrefixToEnvVarConfigSrc(IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration(config =>
            {
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
