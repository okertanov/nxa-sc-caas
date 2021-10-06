using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
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
            CompilerTask resultTask = task;

            var template = Handlebars.Compile(task.Create!.ContractSource);
            var data = new
            {
                SystemOwnerAddress = task.Create.SystemOwnerAddress,
                ContractAuthorAddress = task.Create.ContractAuthorAddress,
                ContractAuthorName = task.Create.ContractAuthorName,
                ContractAuthorEmail = task.Create.ContractAuthorEmail,
                ContractName = task.Create.ContractName,
                ContractDescription = task.Create.ContractDescription,
                ContractSymbol = task.Create.ContractSymbol,
                ContractFactor = task.Create.ContractFactor,
                ContractDecimals = task.Create.ContractDecimals
            };

            var codeStr = template(data);
            var neoRes = Neo.Compiler.CompilerService.Compile(codeStr);

            var neoErrors = neoRes.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();
            if (neoErrors.Count() > 0)
            {
                var firstError = neoErrors.First();
                var errorLine = firstError.Location.GetLineSpan().StartLinePosition.Line;
                var errorCode = int.Parse(firstError.Id.Replace("CS", ""));
                var compilerError = new CompilerError(task.Create.ContractName, (uint)errorLine, errorCode, firstError.GetMessage(), null);
                resultTask = task.SetError(compilerError);
            }
            else
            {
                var compileRes = new CompilerResult(neoRes.Nef.Script, neoRes.Manifest.AsString());
                resultTask = task
                    .SetResult(compileRes);
            }

            return Task.FromResult(resultTask);
        }
    }
}
