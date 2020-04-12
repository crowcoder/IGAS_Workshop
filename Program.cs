using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IGAS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateDefaultBuilderManually(args);
            builder.UseStartup<Startup>();
            builder.Build().Run();
        }

        /// <summary>
        /// Used to demonstrate configuration behavior by allowing you to access
        /// configuration normally abastracted away by default project template.
        /// </summary>
        public static IWebHostBuilder CreateDefaultBuilderManually(string[] args)
        {
            var builder = new WebHostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());

            if (args != null)
            {
                builder.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
            }

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                // To avoid noise in the response, filter environment variables to
                // only include the ones prefixed with IGAS_
                config.AddEnvironmentVariables(prefix: "IGAS_");

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            }).
            UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            });

            ConfigureWebDefaultsManually(builder);

            return builder;
        }

        internal static void ConfigureWebDefaultsManually(IWebHostBuilder builder)
        {
            builder.UseKestrel((builderContext, options) =>
            {
                options.Configure(builderContext.Configuration.GetSection("Kestrel"));
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.PostConfigure<HostFilteringOptions>(options =>
                {
                    options.AllowedHosts = new[] { "*" };
                });

                services.AddRouting();
            });
        }
    }
}
