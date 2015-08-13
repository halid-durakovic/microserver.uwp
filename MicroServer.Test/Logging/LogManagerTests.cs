using System;

using MicroServer.Logging;
using MicroServer.Logging.Loggers;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MicroServer.Logging.Providers;

namespace MicroServer.Test.Logging
{
    [TestClass]
    public class LogManagerTests
    {
        [TestMethod]
        public void default_setting_returns_null_logger_to_get_rid_of_null_checks()
        {
            LogManager.Provider = null;
            var actual = LogManager.GetLogger<LogManagerTests>();

            Assert.IsInstanceOfType(actual, typeof(NullLogger));
        }

        //[TestMethod]
        //public void assigning_a_new_provider_removes_the_old_one()
        //{
        //    var provider = Substitute.For<ILogProvider>();
        //    var expected = Substitute.For<ILogger>();
        //    provider.GetLogger(Arg.Any<Type>()).Returns(expected);

        //    var actual1 = LogManager.GetLogger<LogManagerTests>();
        //    LogManager.Provider = provider;
        //    var actual2 = LogManager.GetLogger<LogManagerTests>();

        //    actual1.Should().BeAssignableTo<NullLogger>();
        //    actual2.Should().Be(expected);
        //}

        //[TestMethod]
        //public void get_logger_using_the_non_generic_method_should_return_expected_logger()
        //{
        //    var provider = Substitute.For<ILogProvider>();
        //    var expected = Substitute.For<ILogger>();
        //    provider.GetLogger(Arg.Any<Type>()).Returns(expected);

        //    LogManager.Provider = provider;
        //    var actual = LogManager.GetLogger(GetType());

        //    actual.Should().Be(expected);
        //}

    }
}
