using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class Test_NodeInterop
    {
        private readonly string scripts = @"../../../../nxa-sc-caas/NodeCode/scripts";

        [TestMethod]
        public void Test_EvalReturnsInvalidDate()
        {
            var param = @"return eval('10+11');";
            var nodeInteropService= NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "momentjs", new[] { param });
            var momentRes = res.Result["dateString"];
            Assert.AreEqual(momentRes, "Invalid date");
        }

        [TestMethod]
        public void Test_MomentReturnsCorrectDate()
        {
            var param = @"554433665";
            var nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "momentjs", new[] { param });
            var momentRes = res.Result["dateString"];
            Assert.AreEqual(momentRes, "1987-07-28");
        }

        [TestMethod]
        public void Test_EvalReturnsCorrectRes()
        {
            var param = @"10+11";
            var nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "evaluate", new[] { param });
            var evalRes = res.Result["result"];
            Assert.AreEqual(evalRes, 21);
        }

        [TestMethod]
        public void Test_EvalInvalidReturnsError()
        {
            var param = @"_ 1";
            var nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "evaluate", new[] { param });
            var error = res.Result["error"];
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void Test_EvalEmptyReturnsNull()
        {
            var param = @"";
            var nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "evaluate", new[] { param });
            var evalRes = res.Result["result"];
            Assert.AreEqual(evalRes, null);
        }

        [TestMethod]
        public void Test_UuidNotEmpty()
        {
            var nodeInteropService = NodeInteropFactory.CreateNodeInteropService();
            var res = nodeInteropService.Execute(scripts, "uuid");
            var evalRes = res.Result["uuid"];
            Assert.IsNotNull(evalRes);
        }
    }
}
