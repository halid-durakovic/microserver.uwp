using MicroServer.Logging.Loggers;
using MicroServer.Logging.Providers;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MicroServer.Test.Logging.Providers
{
    [TestClass]
    public class LogProviderTests
    {
        [TestMethod]
        public void return_the_only_added_logger()
        {
            var sut = new LogProvider();
            sut.Add(new NullLogger());

            var logger = sut.GetLogger(GetType());

            Assert.IsInstanceOfType(logger, typeof(NullLogger));
            //logger.Should().BeOfType<NullLogger>();
        }

        [TestMethod]
        public void return_all_loggers_as_a_composite_if_no_filters_are_used()
        {
            var sut = new LogProvider();
            sut.Add(new NullLogger());
            sut.Add(new NullLogger());

            var logger = sut.GetLogger(GetType());

            //logger.Should().BeOfType<CompositeLogger>();
            Assert.IsInstanceOfType(logger, typeof(CompositeLogger));
        }

        [TestMethod]
        public void only_return_one_logger_if_the_filter_is_not_ok()
        {
            var sut = new LogProvider();
            sut.Add(new NullLogger());
            sut.Add(new NullLogger(), new NamespaceFilter(revokedIncludingChildNamespaces: "MicroServer"));

            var logger = sut.GetLogger(GetType());

            //logger.Should().BeOfType<NullLogger>();
            Assert.IsInstanceOfType(logger, typeof(NullLogger));
        }

        [TestMethod]
        public void return_composite_if_filter_is_ok()
        {
            var sut = new LogProvider();
            sut.Add(new NullLogger());
            sut.Add(new NullLogger(), new NamespaceFilter(allowedIncludingChildNamespaces: "MicroServer"));

            var logger = sut.GetLogger(GetType());

            //logger.Should().BeOfType<CompositeLogger>();
            Assert.IsInstanceOfType(logger, typeof(CompositeLogger));
        }
    }
}
