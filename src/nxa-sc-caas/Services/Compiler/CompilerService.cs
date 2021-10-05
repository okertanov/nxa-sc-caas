using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Compiler.Impl {
    public class CompilerService: ICompilerService {
        private readonly ILogger<CompilerService> _logger;

        public CompilerService(ILogger<CompilerService> logger) {
            _logger = logger;
        }

        public Task<CompilerTask> Compile(CompilerTask task) {
            _logger.LogDebug($"Compiling: {task.Create?.ContractName}...");
            var result = new CompilerResult(new byte[]{}, String.Empty);
            task.SetResult(result);
            return Task.FromResult(task);
        }
    }
}
