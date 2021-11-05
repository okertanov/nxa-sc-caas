using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("")]
    public class ApiRootController : ControllerBase {
        private readonly ILogger<ApiRootController> logger;

        public ApiRootController(ILogger<ApiRootController> logger) {
            this.logger = logger;
        }

        [HttpGet]
        public Object Get() {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");
            
            return new { };
        }
    }
}
