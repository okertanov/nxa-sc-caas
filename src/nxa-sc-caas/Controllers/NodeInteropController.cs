using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NXA.SC.Caas.Services;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class NodeInteropController : ControllerBase
    {
        private readonly ILogger<NodeInteropController> logger;
        private readonly IMediator mediator;
        private readonly string modulePath = "./NodeCode/scripts";

        public NodeInteropController(
            ILogger<NodeInteropController> logger,
            IMediator mediator
        )
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpGet("uuidgen")]
        public async Task<object> GetUuid()
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var executeNodeCommand = new ExecuteNodeCommand { Module = modulePath, Method = "uuid", JSParams = new object[]{ } };
            var res = await mediator.Send(executeNodeCommand);

            return JsonConvert.SerializeObject(res);
        }

        [HttpGet("now")]
        public async Task<object> GetMomentJsDate(string timestamp)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var executeNodeCommand = new ExecuteNodeCommand { Module = modulePath, Method = "momentjs", JSParams = new[] { timestamp } };
            var res = await mediator.Send(executeNodeCommand);

            return JsonConvert.SerializeObject(res);
        }

        [HttpGet("bcrypt")]
        public async Task<object> Bcrypt(string message)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var executeNodeCommand = new ExecuteNodeCommand { Module = modulePath, Method = "bcrypt", JSParams = new[] { message } };
            var res = await mediator.Send(executeNodeCommand);

            return JsonConvert.SerializeObject(res);
        }
    }
}
