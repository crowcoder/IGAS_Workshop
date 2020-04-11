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
          return new string[] {
           $"Testing, Hello World!"
          };
        }
    }
}
