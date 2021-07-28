using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [AllowAnonymous]
    [Route("/")]
    public class ApiRootController : ControllerBase {
        private readonly ILogger<ApiRootController> _logger;

        public ApiRootController(ILogger<ApiRootController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public Object Get() {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path}");
            
            return new { };
        }
    }
}
