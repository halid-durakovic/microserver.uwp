using MicroServer.Logging;
using MicroServer.Logging.Loggers.Filters;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MicroServer.Test.Logging.Loggers.Filters
{
    [TestClass]
    public class LogLevelFilterTests
    {
        [TestMethod]
        public void min_default_level_is_debug()
        {
            var sut = new LogLevelFilter();

            Assert.AreEqual(sut.MinLevel, LogLevel.Debug);
        }

        [TestMethod]
        public void max_default_level_is_error()
        {
            var sut = new LogLevelFilter();

           Assert.AreEqual(sut.MaxLevel, LogLevel.Error);
        }

        [TestMethod]
        public void lower_level_rejects_those_under()
        {
            var sut = new LogLevelFilter();
            sut.MinLevel = LogLevel.Warning;
            var entry = new LogEntry(LogLevel.Info, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void lower_level_is_inclusive()
        {
            var sut = new LogLevelFilter();
            sut.MinLevel = LogLevel.Warning;
            var entry = new LogEntry(LogLevel.Warning, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, true);
        }

        [TestMethod]
        public void lower_level_is_accepting_above_ones()
        {
            var sut = new LogLevelFilter();
            sut.MinLevel = LogLevel.Info;
            var entry = new LogEntry(LogLevel.Warning, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, true);
        }

        [TestMethod]
        public void higher_level_rejects_those_above()
        {
            var sut = new LogLevelFilter();
            sut.MaxLevel = LogLevel.Info;
            var entry = new LogEntry(LogLevel.Warning, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void higher_level_is_inclusive()
        {
            var sut = new LogLevelFilter();
            sut.MaxLevel = LogLevel.Warning;
            var entry = new LogEntry(LogLevel.Warning, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, true);
        }

        [TestMethod]
        public void higher_level_is_accepting_below_ones()
        {
            var sut = new LogLevelFilter();
            sut.MaxLevel = LogLevel.Info;
            var entry = new LogEntry(LogLevel.Debug, "kksdl", null);

            var actual = sut.IsSatisfiedBy(entry);

            Assert.AreEqual(actual, true);
        }
    }
}