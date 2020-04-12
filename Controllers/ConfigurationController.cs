using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
        public IEnumerable<object> Get()
        {
            // For simplicity and to encourage experimentation, this code will retrieve
            // all configuration settings in the system, except for environment variables,
            // which will only be pulled in if prefixed with "IGAS_" because of how the
            // environment variable configuration provider is set up in program.cs. The
            // prefix will be stripped, so you will not see "IGAS_" in your setting name.
            
            var AllConfigSettings = _config.AsEnumerable()
            .Select(c => new { ConfigKey = c.Key, ConfigValue = c.Value});

            return AllConfigSettings;
        }
    }
}
