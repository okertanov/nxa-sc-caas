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

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    public class CompilerController : ControllerBase {
        private readonly ILogger<CompilerController> _logger;
        private readonly ITaskPersistService _taskPersistService;
        private readonly ICompilerService _compilerService;

        public CompilerController(
            ILogger<CompilerController> logger,
            ITaskPersistService taskPersistService,
            ICompilerService compilerService
        ) {
            _logger = logger;
            _taskPersistService = taskPersistService;
            _compilerService = compilerService;
        }

        [HttpGet]
        public async Task<IEnumerable<CompilerTask>> Get(
            [FromQuery(Name = "offset")] int? offset,
            [FromQuery(Name = "limit")] int? limit
        ) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path}");

            var result = await _taskPersistService.GetAll(offset, limit);
            return result;
        }

        [HttpGet("{identifier}")]
        public async Task<CompilerTask> GetByIdentifier(string identifier) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var result = await _taskPersistService.GetByIdentifier(identifier);
            return result;
        }

        [HttpPut]
        public async Task<CompilerTask> Put(CreateCompilerTask dto) {
            var request = HttpContext.Request;
            var dtoStr = JsonSerializer.Serialize(dto);
            _logger.LogTrace($"{request.Method} {request.Path} - {dtoStr}");
            
            // TODO: Introduce Command + Mediator and remove from here
            var stored = await _taskPersistService.Store(dto);

            // TODO: Move to background cycle to break-down this SYNC pipeline to ASYNC one
            var compiled = await _compilerService.Compile(stored);

            var updated = await _taskPersistService.Update(compiled);

            return updated;
        }

        [HttpDelete("{identifier}")]
        public async Task<CompilerTask> DeleteByIdentifier(string identifier) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var result = await _taskPersistService.DeleteByIdentifier(identifier);
            return result;
        }
    }
}
