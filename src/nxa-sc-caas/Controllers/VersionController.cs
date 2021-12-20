using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    public class VersionController : ControllerBase 
    {
        private readonly ILogger<VersionController> logger;

        public VersionController(
            ILogger<VersionController> logger
        )
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult GetServiceVersion()
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");
            return Ok(new ServiceVersion());
        }
    }
}
