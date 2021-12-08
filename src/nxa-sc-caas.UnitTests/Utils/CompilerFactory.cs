using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Compiler.Impl;
using NXA.SC.Caas.Services.Db;
using NXA.SC.Caas.Services.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace nxa_sc_caas.UnitTests
{
    public class CompilerFactory
    {
        private static ILogger<CSharpCompilerService> _logger { get { return new LoggerFactory().CreateLogger<CSharpCompilerService>(); } set { } }
        private static ILogger<SolidityCompilerService> _loggerSolidity { get { return new LoggerFactory().CreateLogger<SolidityCompilerService>(); } set { } }

        public static SolidityCompilerService CreateCompilerServiceSolidity()
        {
            var compilerService = new SolidityCompilerService(_loggerSolidity);
            return compilerService;
        }
        public static CSharpCompilerService CreateCompilerServiceCSharp()
        {
            var compilerService = new CSharpCompilerService(_logger);
            return compilerService;
        }
        public static CompilerTask GetValidSmartContractTask() 
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = @"
                using Neo;
                using Neo.SmartContract;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;
                namespace ProjectName
                {
                    public class Contract1 : SmartContract
                    {
                        private static int privateMethod()
                        {
                            return 1;
                        }
                    }
                }",
                CompilerTaskType = CompilerTaskTypeEnum.CSHARP
            };
            var task = new CompilerTask("123", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetInvalidSmartContractTask()
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = "invalidcontract", CompilerTaskType = CompilerTaskTypeEnum.CSHARP };
            var task = new CompilerTask("1234", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetSolidityContractTask()
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = "invalidcontract", CompilerTaskType = CompilerTaskTypeEnum.SOLIDITY };
            var task = new CompilerTask("1234", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
    }
}
