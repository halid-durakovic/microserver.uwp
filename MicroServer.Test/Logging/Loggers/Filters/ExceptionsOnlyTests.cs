using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using MicroServer.Logging;
using MicroServer.Logging.Loggers.Filters;

namespace MicroServer.Test.Logging.Loggers.Filters
{
    [TestClass]
    public class ExceptionsOnlyTests
    {
        [TestMethod]
        public void reject_entry_without_exception()
        {
            var sut = new ExceptionsOnly();
            var entry = new LogEntry(LogLevel.Trace, "kkjlsdfkl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void accept_entries_that_got_an_exception()
        {
            var sut = new ExceptionsOnly();
            var entry = new LogEntry(LogLevel.Trace, "kkjlsdfkl", new Exception());

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, true);
        }
    }
}
