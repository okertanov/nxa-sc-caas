using System.Threading.Tasks;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Persist
{
    public interface ITaskPersistService
    {
        Task<CompilerTask[]> GetAll(int? offset, int? limit);
        Task<CompilerTask?> GetByIdentifier(string identifier);
        Task<CompilerTask> Store(CreateCompilerTask task, bool asyncCompilation);
        Task<CompilerTask> Update(CompilerTask task, bool asyncCompilation);
        Task<CompilerTask> DeleteByIdentifier(string identifier);
    }
}
