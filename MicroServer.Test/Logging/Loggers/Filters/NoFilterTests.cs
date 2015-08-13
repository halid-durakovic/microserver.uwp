using MicroServer.Logging;
using MicroServer.Logging.Loggers.Filters;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MicroServer.Test.Logging.Loggers.Filters
{
    [TestClass]
    public class NoFilterTests
    {
        [TestMethod]
        public void should_always_accept_everything()
        {
            var sut = NoFilter.Instance;

            var actual = sut.IsSatisfiedBy(new LogEntry(LogLevel.Trace, "kjjklsdf", null));

            Assert.AreEqual(actual, true);
        }
    }
}
