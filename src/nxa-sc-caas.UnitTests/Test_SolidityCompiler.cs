using Microsoft.VisualStudio.TestTools.UnitTesting;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services;
using NXA.SC.Caas.Shared.Utils;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class Test_SolidityCompiler
    {
        private readonly string scripts = @"../../../../nxa-sc-caas/NodeCode/scripts";
        private readonly string taskPath = @"./testContract";
        private readonly INodeInteropService nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
        private readonly CompilerTask validTask = CompilerFactory.GetSolidityValidContractTask();
        private readonly CompilerTask invalidTask = CompilerFactory.GetSolidityInvalidContractTask();

        [TestMethod]
        public void Test_CorrectContractName()
        {
            var contractName = validTask.Create.ContractSource.GetSolContractName();
            Assert.AreEqual(contractName, "TantalisNFT");
        }

        [TestMethod]
        public async Task Test_DirectoryCreatedAndDeleted()
        {
            var contractName = validTask.Create.ContractSource.GetSolContractName();
            await FsHelper.PrepareDirForHhCompile(taskPath, contractName, validTask.Create.ContractSource);
            Assert.IsTrue(Directory.Exists(taskPath));
            await FsHelper.DeleteFolder(taskPath);
            Assert.IsFalse(Directory.Exists(taskPath));
        }

        [TestMethod]
        public async Task Test_HardhatCompilesAndCleansDir()
        {
            var contractName = validTask.Create.ContractSource.GetSolContractName();
            await FsHelper.PrepareDirForHhCompile(taskPath, contractName, validTask.Create.ContractSource);

            var res = await nodeInteropService.Execute(scripts, "hardhatCompile", new[] { contractName, taskPath });
            var evalRes = res["bytecode"];
            await FsHelper.DeleteFolder(taskPath);

            Assert.IsNotNull(evalRes);
            Assert.IsFalse(Directory.Exists(taskPath));
        }

        [TestMethod]
        public async Task Test_HardhatFails()
        {
            var contractName = invalidTask.Create.ContractSource.GetSolContractName();
            await FsHelper.PrepareDirForHhCompile(taskPath, contractName, invalidTask.Create.ContractSource);

            var res = await nodeInteropService.Execute(scripts, "hardhatCompile", new[] { contractName, taskPath });
            var errorRes = res["error"];
            await FsHelper.DeleteFolder(taskPath);

            Assert.IsNotNull(errorRes);
        }
    }
}
