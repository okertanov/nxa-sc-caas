using System.Threading.Tasks;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Compiler
{
    public interface ICompilerService
    {
        Task<CompilerTask> Compile(CompilerTask task);
    }
}
