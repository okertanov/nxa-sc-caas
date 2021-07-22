using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Persist.Impl {
    public class TaskPersistService: ITaskPersistService {
        private readonly ILogger<TaskPersistService> _logger;

        public TaskPersistService(ILogger<TaskPersistService> logger) {
            _logger = logger;
        }

        public Task<CompilerTask[]> GetAll(int? offset, int? limit) {
            var result = new [] { new CompilerTask() };
            return Task.FromResult(result);
        }

        public Task<CompilerTask> GetByIdentifier(string identifier) {
            var result = new CompilerTask();
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Store(CreateCompilerTask task) {
            _logger.LogDebug($"Storing: {task.ContractName}...");

            var result = new CompilerTask("", CompilerTaskStatus.CREATED, task, null, null);
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Update(CompilerTask task) {
            _logger.LogDebug($"Updating: {task.Create?.ContractName}...");

            return Task.FromResult(task);
        }

        public Task<CompilerTask> DeleteByIdentifier(string identifier) {
            var result = new CompilerTask();
            return Task.FromResult(result);
        }
    }
}
