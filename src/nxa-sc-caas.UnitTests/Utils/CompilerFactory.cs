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
        private static ILogger<CompilerService> _logger { get { return new LoggerFactory().CreateLogger<CompilerService>(); } set { } }

        public static CompilerService CreateCompilerService()
        {
            var compilerService = new CompilerService(_logger);
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
                }" 
            };
            var task = new CompilerTask("123", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetInvalidSmartContractTask()
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = "invalidcontract" };
            var task = new CompilerTask("1234", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
    }
}
