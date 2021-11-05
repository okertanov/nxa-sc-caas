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

namespace NXA.SC.Caas.Services.Compiler.Impl
{
    public class CompilerService : ICompilerService
    {
        private readonly ILogger<CompilerService> logger;

        public CompilerService(ILogger<CompilerService> logger)
        {
            this.logger = logger;
        }

        public Task<CompilerTask> Compile(CompilerTask task)
        {
            logger.LogDebug($"Compiling: {task.Create?.ContractName}...");
            
            var resultTask = task;
            var neoRes = new Neo.Compiler.CompileResult();
            var sourceStr = task.Create!.ContractSource;
            var sourceStrNormalized = sourceStr.IsBase64String() ? Encoding.UTF8.GetString(Convert.FromBase64String(sourceStr)) : sourceStr;
            var template = Handlebars.Compile(sourceStrNormalized);
            
            var data = new
            {
                SystemOwnerAddress = task.Create.SystemOwnerAddress,
                ContractAuthorAddress = task.Create.ContractAuthorAddress,
                ContractAuthorName = task.Create.ContractAuthorName,
                ContractAuthorEmail = task.Create.ContractAuthorEmail,
                ContractName = task.Create.ContractName,
                ContractDescription = task.Create.ContractDescription,
                ContractSymbol = task.Create.ContractSymbol,
                ContractDecimals = task.Create.ContractDecimals,
                ContractInitialCoins = task.Create.ContractInitialCoins
            };

            var codeStr = template(data);

            try
            {
                neoRes = Neo.Compiler.CompilerService.Compile(codeStr);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);

                var stackTrace = new StackTrace(ex, true);
                var line = stackTrace?.GetFrame(0)?.GetFileLineNumber() ?? 0;
                var compilerError = new CompilerError(task.Create.ContractName, (uint)line, ex.HResult.ToString(), ex.Message, ex.StackTrace);
                resultTask = task.SetError(compilerError);
                return Task.FromResult(resultTask);
            }

            var neoErrors = neoRes.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();
            
            if (neoErrors.Count() > 0)
            {
                var firstError = neoErrors.First();
                var errorLine = firstError.Location.GetLineSpan().StartLinePosition.Line;
                var compilerError = new CompilerError(task.Create.ContractName, (uint)errorLine, firstError.Id, firstError.GetMessage(), null);
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
        private readonly ICompilerService _compilerService;
        public CompileCommandHandler(ICompilerService compilerService)
        {
            _compilerService = compilerService;
        }

        public Task<CompilerTask> Handle(CompileCommand request, CancellationToken cancellationToken)
        {
            return _compilerService.Compile(request.Task);
        }
    }

    public struct CompileCommand : IRequest<CompilerTask>
    {
        public CompilerTask Task { get; set; }
    }
}
