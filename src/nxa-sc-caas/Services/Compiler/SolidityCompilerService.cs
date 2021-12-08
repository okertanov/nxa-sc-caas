using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Compiler.Impl
{
    public class SolidityCompilerService : ICompilerService
    {
        private readonly ILogger<SolidityCompilerService> logger;

        public SolidityCompilerService(ILogger<SolidityCompilerService> logger)
        {
            this.logger = logger;
        }
        public Task<CompilerTask> Compile(CompilerTask task)
        {
            throw new NotImplementedException();
        }
    }
}
