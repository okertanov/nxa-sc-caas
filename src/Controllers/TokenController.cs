using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Persist;
using NXA.SC.Caas.Services.Compiler;
using Microsoft.AspNetCore.Authorization;
using NXA.SC.Caas.Services.Token;

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [AllowAnonymous]
    [Route("token")]
	public class TokenController : ControllerBase 
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenService _tokenService;

        public TokenController(
            ILogger<TokenController> logger,
            ITokenService tokenService
        )
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginInfo loginInput)
        {
            var request = HttpContext.Request;
            IActionResult result = Unauthorized();
            _logger.LogTrace($"{request.Method} {request.Path} - {JsonSerializer.Serialize(loginInput)}");

            if (loginInput.Valid)
                result = Ok(new { token = _tokenService.GenerateWebToken() });

            return result;
        }

    }
}
