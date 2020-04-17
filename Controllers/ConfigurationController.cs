using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;

namespace IGAS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        IConfiguration _config;
        public ConfigurationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IEnumerable<ProviderViewModel> Get()
        {
            // For simplicity and to encourage experimentation, this code will retrieve
            // all configuration settings in the system, except for environment variables,
            // which will only be pulled in if prefixed with "IGAS_" because of how the
            // environment variable configuration provider is set up in program.cs. The
            // prefix will be stripped, so you will not see "IGAS_" in your setting name.

            var providers = ((IConfigurationRoot)_config).Providers;

            List<ProviderViewModel> providerViewModels = new List<ProviderViewModel>();

            foreach (var provider in providers)
            {
                ProviderViewModel mdl = new ProviderViewModel();
                mdl.ProviderName = provider.GetType().Name;

                switch (provider)
                {
                    case JsonConfigurationProvider j:
                        mdl.Source = j.Source.Path;
                        mdl.ConfigValues = GetProtectedData<JsonConfigurationProvider>(j);
                        break;
                    case EnvironmentVariablesConfigurationProvider e:

                        break;
                    default:
                        break;
                }

                providerViewModels.Add(mdl);
            }

            // var AllConfigSettings = _config.AsEnumerable()
            // .Select(c => new { ConfigKey = c.Key, ConfigValue = c.Value});

            return providerViewModels;
        }

        private Dictionary<string, string> GetProtectedData<T>(Object instance)
        {
            PropertyInfo pInfo = typeof(T).GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance);

            if (pInfo != null)
            {
                var data = pInfo.GetValue(instance) as IDictionary<string, string>;
                return data.ToDictionary(d => d.Key, d => d.Value);
            }
            return null;
        }
    }
}
