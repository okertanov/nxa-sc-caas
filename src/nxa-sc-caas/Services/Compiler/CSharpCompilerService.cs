using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Extensions;
using Neo.IO;
using MediatR;
using System.Threading;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace NXA.SC.Caas.Services.Compiler.Impl
{
    public class CSharpCompilerService : ICompilerService
    {
        private readonly ILogger<CSharpCompilerService> logger;

        public CSharpCompilerService(ILogger<CSharpCompilerService> logger)
        {
            this.logger = logger;
        }
        
        public Task<CompilerTask> Compile(CompilerTask task)
        {
            var contractName = task.Create.GetNamedContractVal(ContractValueEnum.ContractName).ToString();
            if (string.IsNullOrEmpty(contractName))
            {
                throw new ArgumentNullException(nameof(contractName));
            }

            logger.LogDebug($"Compiling: {contractName}...");
            
            var resultTask = task;
            var neoRes = new Neo.Compiler.CompileResult();
            var sourceStr = task.Create!.ContractSource;
            var sourceStrNormalized = sourceStr.IsBase64String() ? Encoding.UTF8.GetString(Convert.FromBase64String(sourceStr)) : sourceStr;
            var template = Handlebars.Compile(sourceStrNormalized);
            var codeStr = template(task.Create.ContractValues);

            try
            {
                neoRes = Neo.Compiler.CompilerService.Compile(codeStr);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);

                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace?.GetFrame(0)?.GetFileLineNumber() ?? 0;
                var compilerError = new CompilerError(contractName, (uint)line, ex.HResult.ToString(), ex.Message, ex.StackTrace);
                resultTask = task.SetError(compilerError);
                return Task.FromResult(resultTask);
            }

            var neoErrors = neoRes.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();
            
            if (neoErrors.Count() > 0)
            {
                var firstError = neoErrors.First();
                var errorLine = firstError.Location.GetLineSpan().StartLinePosition.Line;
                var compilerError = new CompilerError(contractName, (uint)errorLine, firstError.Id, firstError.GetMessage(), null);
                resultTask = task.SetError(compilerError);
            }
            else
            {
                var compileRes = new CompilerResult(neoRes.Nef.Script, neoRes.Nef.ToArray(), neoRes.Manifest.AsString());
                resultTask = task
                    .SetResult(compileRes);
            }
            return Task.FromResult(resultTask);
        }
    }

    public class CompileCommandHandler : IRequestHandler<CompileCommand, CompilerTask>
    {
        private readonly IServiceProvider serviceProvider;
        private ICompilerService compilerService;
        public CompileCommandHandler(IServiceProvider serviceProvider, ICompilerService compilerService)
        {
            this.serviceProvider = serviceProvider;
            this.compilerService = compilerService;
        }

        public Task<CompilerTask> Handle(CompileCommand request, CancellationToken cancellationToken)
        {
            var services = serviceProvider.GetServices<ICompilerService>();

            switch (request.Task.Create.CompilerTaskType)
            {
                case CompilerTaskTypeEnum.SOLIDITY:
                    compilerService = services.First(s => s.GetType() == typeof(SolidityCompilerService));
                    break;
                case CompilerTaskTypeEnum.CSHARP:
                    compilerService = services.First(s => s.GetType() == typeof(CSharpCompilerService));
                    break;
                default:
                    throw new NotSupportedException();
            }

            return compilerService.Compile(request.Task);
        }
    }

    public struct CompileCommand : IRequest<CompilerTask>
    {
        public CompilerTask Task { get; set; }
    }
}
