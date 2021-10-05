using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    public class VersionController : ControllerBase 
    {
        private readonly ILogger<StatusController> _logger;

        public VersionController(
            ILogger<StatusController> logger
        )
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetServiceVersion()
        {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path}");

            return Ok(new ServiceVersion());
        }

    }
}
