using System.Threading.Tasks;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Persist {
    public interface ITaskPersistService {
        Task<CompilerTask[]> GetAll(int? offset, int? limit);
        Task<CompilerTask> GetByIdentifier(string identifier);
        Task<CompilerTask> Store(CreateCompilerTask task);
        Task<CompilerTask> Update(CompilerTask task);
        Task<CompilerTask> DeleteByIdentifier(string identifier);
    }
}
