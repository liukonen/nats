using nats_cli_framework;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nats_cli_framework.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        [TestMethod()]
        public void MainTest()
        {
            Assert.Fail();
        }
    }
}

namespace cliFrameworkTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            //NATS -P path -K keyword [Search Type] [options]

            string[] args = new string[] { "-P", @"C:\Users\liuko\Documents", "-k","luke", "-W" };

            nats_cli_framework.Program.Main(args);
        }
    }
}
