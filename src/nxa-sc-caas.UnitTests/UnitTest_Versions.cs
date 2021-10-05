using Microsoft.VisualStudio.TestTools.UnitTesting;
using NXA.SC.Caas.Models;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class UnitTest_Versions
    {
        [TestMethod]
        public void Test_CorrectVersions()
        {
            var serviceVersion = new ServiceVersion();
            Assert.AreEqual(serviceVersion.Abi, "1.2.3");
            Assert.AreEqual(serviceVersion.Api, "1.0.0");
            Assert.AreEqual(serviceVersion.Compiler, "3.0.1");
            Assert.AreEqual(serviceVersion.Flamework, "3.2.1");
        }
    }
}
