using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Shared.Utils;

namespace NXA.SC.Caas.Services.Compiler.Impl
{
    public class SolidityCompilerService : ICompilerService
    {
        private readonly ILogger<SolidityCompilerService> logger;
        private readonly IMediator mediator;

        public SolidityCompilerService(ILogger<SolidityCompilerService> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }
        public async Task<CompilerTask> Compile(CompilerTask task)
        {
            var resultTask = task;
            var taskPath = $@"./{task.Identifier}";
            var sourceStrNormalized = task.Create.ContractSource.IsBase64String() ? Encoding.UTF8.GetString(Convert.FromBase64String(task.Create.ContractSource)) : task.Create.ContractSource;
            var contractName = sourceStrNormalized.GetSolContractName();
            if (string.IsNullOrEmpty(contractName))
                throw new ArgumentNullException(nameof(contractName));

            await FsHelper.PrepareDirForHhCompile(taskPath, contractName, sourceStrNormalized);
            var executeNodeCommand = new ExecuteNodeCommand { Module = "./NodeCode/scripts", Method = "hardhatCompile", JSParams = new object[] { contractName, taskPath } };
            var res = await mediator.Send(executeNodeCommand);

            if (res.ContainsKey("error"))
            {
                var compilerError = new CompilerError(contractName, default(int), String.Empty, res.ToString(), null);
                resultTask = resultTask.SetError(compilerError);
            }
            else
            {
                var resByteCode = res["bytecode"]?.ToString();
                var resAbi = res["abi"]?.ToString();
                if (string.IsNullOrEmpty(resByteCode) || string.IsNullOrEmpty(resAbi))
                {
                    var compilerError = new CompilerError(contractName, default(int), String.Empty, "No bytecode/abi in result", null);
                    resultTask.SetError(compilerError);
                }
                resByteCode = resByteCode!.PadBase64String();
                var resByteArr = Convert.FromBase64String(resByteCode!);
                var compileRes = new CompilerResult(resByteArr, Array.Empty<byte>(), JsonConvert.SerializeObject(resAbi!));
                resultTask = task
                    .SetResult(compileRes);
            }

            await FsHelper.DeleteFolder(taskPath);
            return resultTask;
        }
    }
}
