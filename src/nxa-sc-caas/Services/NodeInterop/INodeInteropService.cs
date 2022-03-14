using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Services
{
    public interface INodeInteropService
    {
        Task<JObject> Execute(string module, string code, params object[] jsParams);
    }
}
