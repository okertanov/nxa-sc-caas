using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class UnitTest_TemplatePreprocess
    {
        private readonly string source = @"../../../TestTokens/NFTContractForTests.cs";
        private TemplatePreprocessService templateService = new TemplatePreprocessService(new LoggerFactory().CreateLogger<TemplatePreprocessService>());

        private List<TemplateParam> GetTestContractParameters()
        {
            var srcText = File.ReadAllText(source);
            return templateService.FindParams(srcText, string.Empty).Result.ToList();
        }

        [TestMethod]
        public void Test_SourceFileExists()
        {
            var srcText= File.ReadAllText(source);
            Assert.IsTrue(srcText.Length > 0);
        }

        [TestMethod]
        public void Test_ReturnsCorrectCount()
        {
            var contractParams = GetTestContractParameters();
            var paramCount = contractParams.Count();

            Assert.AreEqual(paramCount, 10);
        }

        [TestMethod]
        public void Test_DisplayNameIsString()
        {
            var contractParams = GetTestContractParameters();
            var displayName = contractParams.FirstOrDefault(c => c.Name == "DisplayName");

            Assert.AreEqual(displayName.Type, "string");
        }

        [TestMethod]
        public void Test_SymbolIsString()
        {
            var contractParams = GetTestContractParameters();
            var symbol = contractParams.FirstOrDefault(c => c.Name == "TokenSymbol");

            Assert.AreEqual(symbol.Type, "string");
        }

        [TestMethod]
        public void Test_AdminAddressIsUInt()
        {
            var contractParams = GetTestContractParameters();
            var contractAdmin = contractParams.FirstOrDefault(c => c.Name == "ContractAdminAddress");

            Assert.AreEqual(contractAdmin.Type, "UInt160");
        }

        [TestMethod]
        public void Test_OwnerIsUInt()
        {
            var contractParams = GetTestContractParameters();
            var owner = contractParams.FirstOrDefault(c => c.Name == "Owner");

            Assert.AreEqual(owner.Type, "UInt160");
        }

        [TestMethod]
        public void Test_TwoEmails()
        {
            var contractParams = GetTestContractParameters();
            var emails = contractParams.FindAll(c => c.Name == "Email");

            Assert.AreEqual(emails.Count, 2);
        }

        [TestMethod]
        public void Test_DisplayNameCorrectSource()
        {
            var contractParams = GetTestContractParameters();
            var displayName = contractParams.FirstOrDefault(c => c.Name == "DisplayName");

            Assert.AreEqual(displayName.Source.Line, 8);
            Assert.AreEqual(displayName.Source.Column, 17);
        }

        [TestMethod]
        public void Test_DisplayNameCorrectValidation()
        {
            var contractParams = GetTestContractParameters();
            var displayName = contractParams.FirstOrDefault(c => c.Name == "DisplayName");

            var isString = displayName.Validation.DefaultValue is string;
            Assert.IsTrue(isString);
        }
    }
}
