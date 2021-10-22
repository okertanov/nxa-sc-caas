﻿using System;
using System.Linq;
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

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    public class CompilerController : ControllerBase {
        private readonly ILogger<CompilerController> _logger;
        private readonly IMediator _mediator;

        public CompilerController(IMediator mediator,
            ILogger<CompilerController> logger
        ) {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<CompilerTask>> Get(
            [FromQuery(Name = "offset")] int? offset,
            [FromQuery(Name = "limit")] int? limit
        ) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path}");

            var command = new GetAllTasksCommand { Offset = offset, Limit = limit };
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet("{identifier}")]
        public async Task<CompilerTask> GetByIdentifier(string identifier) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var command = new GetTasksByIdCommand { Identifier = identifier };
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPut]
        public async Task<CompilerTask> Put(CreateCompilerTask dto) {
            var request = HttpContext.Request;
            var dtoStr = JsonSerializer.Serialize(dto);
            _logger.LogTrace($"{request.Method} {request.Path} - {dtoStr}");

            var storeCommand = new StoreTasksCommand { Task = dto };
            var stored = await _mediator.Send(storeCommand);

            // TODO: Move to background cycle to break-down this SYNC pipeline to ASYNC one
            var compileCommand = new CompileCommand { Task = stored };
            var compiled = await _mediator.Send(compileCommand);

            var updateCommand = new UpdateTasksCommand { Task = compiled };
            var updated = await _mediator.Send(updateCommand);

            return updated;
        }

        [HttpDelete("{identifier}")]
        public async Task<CompilerTask> DeleteByIdentifier(string identifier) {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path} - {identifier}");

            var command = new DeleteTasksByIdCommand { Identifier = identifier };
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
