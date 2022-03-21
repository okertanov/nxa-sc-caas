using Microsoft.VisualStudio.TestTools.UnitTesting;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Compiler.Impl;
using System;
using System.Collections.Generic;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class UnitTest_CSharpCompiler
    {
        [TestMethod]
        public void Test_SetsIdentifier()
        {
            var task = new CompilerTask();
            var newTask = task.SetIdentifier("id1");
            Assert.AreEqual(newTask.Identifier, "id1");
        }
        [TestMethod]
        public void Test_SetsStatus()
        {
            var task = new CompilerTask();
            var newTask = task.SetStatus(CompilerTaskStatus.CREATED);
            Assert.AreEqual(newTask.Status, CompilerTaskStatus.CREATED);
        }
        [TestMethod]
        public void Test_SetsCreate()
        {
            var task = new CompilerTask();
            var taskContractVals = new Dictionary<string, object> 
            { 
                { ContractValueEnum.ContractAuthorName.ToString(), "name1" },
                { ContractValueEnum.ContractName.ToString(), "name12" },
            };
            var create = new CreateCompilerTask { ContractValues = taskContractVals };
            var newTask = task.SetCreate(create);
            Assert.AreEqual(newTask.Create.GetNamedContractVal(ContractValueEnum.ContractAuthorName), "name1");
        }
        [TestMethod]
        public void Test_SetsResult()
        {
            var task = new CompilerTask();
            var result = new CompilerResult(new byte[64], new byte[64], "manifestjson");
            var newTask = task.SetResult(result);
            Assert.AreEqual(newTask.Result.Manifest, "manifestjson");
        }
        [TestMethod]
        public void Test_SetsError()
        {
            var task = new CompilerTask();
            var error = new CompilerError("filestr", (uint)1, "123", "errormsg", string.Empty);
            var newTask = task.SetError(error);
            Assert.AreEqual(newTask.Error.Messsage, "errormsg");
        }
        [TestMethod]
        public void Test_CompileError()
        {
            var compilerService = CompilerFactory.CreateCompilerServiceCSharp();
            var task = CompilerFactory.GetInvalidSmartContractTask();
            var result = compilerService.Compile(task);
            Assert.IsNotNull(result.Result.Error);
            Assert.IsNull(result.Result.Result);
        }
        [TestMethod]
        public void Test_CompileResultValid()
        {
            var compilerService = CompilerFactory.CreateCompilerServiceCSharp();
            var task = CompilerFactory.GetValidSmartContractTask();
            var result = compilerService.Compile(task);
            Assert.IsNull(result.Result.Error);
            Assert.IsNotNull(result.Result.Result);
        }
        [TestMethod]
        public void Test_ContractCustomValueAddedString()
        {
            var task = new CompilerTask();
            var taskContractVals = new Dictionary<string, object>
            {
                { "testParam", "testParamVal" },
                { ContractValueEnum.ContractName.ToString(), "name12" },
            };
            var create = new CreateCompilerTask { ContractValues = taskContractVals };
            var newTask = task.SetCreate(create);
            Assert.AreEqual(newTask.Create.ContractValues["testParam"], "testParamVal");
        }
        [TestMethod]
        public void Test_ContractCustomValueAddedInt()
        {
            var task = new CompilerTask();
            var taskContractVals = new Dictionary<string, object>
            {
                { "testParamInt", 1 },
                { ContractValueEnum.ContractName.ToString(), "name12" },
            };
            var create = new CreateCompilerTask { ContractValues = taskContractVals };
            var newTask = task.SetCreate(create);
            Assert.AreEqual(newTask.Create.ContractValues["testParamInt"], 1);
        }
        [TestMethod]
        public void Test_ContractClassNameError()
        {
            var compilerService = CompilerFactory.CreateCompilerServiceCSharp();
            var task = CompilerFactory.GetSmartContractTaskWithClassNameAsInput();
            var result = compilerService.Compile(task);
            Assert.IsNotNull(result.Result.Error);
            Assert.AreEqual(result.Result.Error.Code, "CS1514");
        }
    }
}
