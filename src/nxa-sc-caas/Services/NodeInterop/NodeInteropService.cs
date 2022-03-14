using MediatR;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Services
{
    public class NodeInteropService : INodeInteropService
    {
        private readonly ILogger<NodeInteropService> logger; 
        private readonly INodeServices nodeServices;

        public NodeInteropService(ILogger<NodeInteropService> logger, INodeServices nodeServices)
        {
            this.logger = logger;
            this.nodeServices = nodeServices;
        }

        public async Task<JObject> Execute(string module, string method, params object[] jsParams)
        {
            try
            {
                //
                // invoke method that is defined in scripts.js file under "module.exports = {"
                //
                logger.LogInformation($"Invoking method {method}");
                var res = await nodeServices.InvokeExportAsync<JObject>(module, method, jsParams);
                logger.LogInformation(res.ToString());
                return res;
            }
            catch (System.Exception e)
            {
                return new JObject(new JProperty("error", e.Message));
            }
        }
    }

    public struct ExecuteNodeCommand : IRequest<JObject>
    {
        public string Method { get; set; }
        public string Module { get; set; }
        public object[] JSParams { get; set; }
    }

    public class ExecuteNodeHandler : IRequestHandler<ExecuteNodeCommand, JObject>
    {
        private readonly INodeInteropService nodeInteropService;

        public ExecuteNodeHandler(INodeInteropService nodeInteropService)
        {
            this.nodeInteropService = nodeInteropService;
        }

        public async Task<JObject> Handle(ExecuteNodeCommand request, CancellationToken cancellationToken)
        {
            var result = await nodeInteropService.Execute(request.Module, request.Method, request.JSParams);
            return result;
        }
    }
}
