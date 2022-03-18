using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using NXA.SC.Caas.Services.Persist.Impl;
using NXA.SC.Caas.Services.Compiler.Impl;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    public class CompilerController : ControllerBase
    {
        private readonly ILogger<CompilerController> logger;
        private readonly IMediator mediator;

        public CompilerController(
            IMediator mediator,
            ILogger<CompilerController> logger
        )
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<CompilerTask>> Get(
            [FromQuery(Name = "offset")] int? offset,
            [FromQuery(Name = "limit")] int? limit
        )
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var command = new GetAllTasksCommand { Offset = offset, Limit = limit };
            var result = await mediator.Send(command);
            return result;
        }

        [HttpGet("{identifier}")]
        public async Task<CompilerTask> GetByIdentifier(string identifier)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var command = new GetTasksByIdCommand { Identifier = identifier };
            var result = await mediator.Send(command);
            return result;
        }

        [HttpPut]
        public async Task<CompilerTask> Put(CreateCompilerTask dto)
        {
            var request = HttpContext.Request;
            var asyncCompilation = request.Headers["AsynchronousCompilation"].ToString() == "true";

            var dtoStr = JsonSerializer.Serialize(dto);
            logger.LogTrace($"{request.Method} {request.Path} - {dtoStr}");

            var storeCommand = new StoreTasksCommand { Task = dto, AsyncCompilation = asyncCompilation };
            var stored = await mediator.Send(storeCommand);

            if (!asyncCompilation)
            {
                var compileCommand = new CompileCommand { Task = stored };
                var compiled = await mediator.Send(compileCommand);

                var updateCommand = new UpdateTasksCommand { Task = compiled, AsyncCompilation = asyncCompilation };
                var updated = await mediator.Send(updateCommand);

                return updated;
            }

            return stored;
        }

        [HttpDelete("{identifier}")]
        public async Task<CompilerTask> DeleteByIdentifier(string identifier)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var command = new DeleteTasksByIdCommand { Identifier = identifier };
            var result = await mediator.Send(command);
            return result;
        }
    }
}
