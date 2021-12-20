using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas;
using NXA.SC.Caas.Services;

namespace nxa_sc_caas.UnitTests
{
    public class NodeInteropFactory
    {
        public static INodeInteropService CreateNodeInteropService()
        {
            var hostBuildes = Program.CreateHostBuilder(new string[] { });
            var host = hostBuildes.Build();
            var nodeInteropService = host.Services.GetRequiredService<INodeInteropService>();
            return nodeInteropService;
        }
    }
}
