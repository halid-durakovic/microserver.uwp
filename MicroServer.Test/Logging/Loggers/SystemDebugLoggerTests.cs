using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using MicroServer.Logging.Loggers;

namespace MicroServer.Test.Tests.Logging.Loggers
{
    [TestClass]
    public class SystemDebugLoggerTests
    {
        [TestMethod]
        public void Just_verify_the_regular_write()
        {

            var sut = new SystemDebugLogger(GetType());
            sut.Debug("Hello world");

        }

        [TestMethod]
        public void Just_verify_formatted_Write()
        {

            var sut = new SystemDebugLogger(GetType());
            sut.Debug("Hello {0}", "world");

        }

        [TestMethod]
        public void Just_verify_exception_Write()
        {
            try
            {
                var sut = new SystemDebugLogger(GetType());
                sut.Debug("Hello world", new Exception());
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
            }
        }
    }
}
