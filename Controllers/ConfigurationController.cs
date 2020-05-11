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
        public IEnumerable<string> Get()
        {
            // replace from here...
            string envVar = _config.GetValue<string>("TMP") ?? "NOT SET";
            string userSecret = _config.GetValue<string>("MyUserSecret") ?? "NOT SET";
            string appSetting = _config.GetValue<string>("FromAppSettings") ?? "NOT SET";

            return new string[] {
        $"Environment Variable TMP = {envVar}",
        $"User Secret MyUserSecret = {userSecret}",
        $"App Setting FromAppSettings = {appSetting}"
    };
            // .. to here
        }
    }
}
