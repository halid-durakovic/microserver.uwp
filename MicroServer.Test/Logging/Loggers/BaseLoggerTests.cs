using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using MicroServer.Logging;
using System.Diagnostics;

namespace MicroServer.Test.Logging.Loggers
{
    [TestClass]
    public class BaseLoggerTests
    {

        [TestMethod]
        public void trace_without_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Trace("Hello");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Trace);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void trace_formatted()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Trace("Hello {0}", "World");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello World");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Trace);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void trace_with_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Trace("Hello", new ExternalException());

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Trace);
            Assert.IsInstanceOfType(sut.Entries[0].Exception, typeof(ExternalException));
        }


        [TestMethod]
        public void debug_without_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Debug("Hello");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Debug);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void debug_formatted()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Debug("Hello {0}", "World");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello World");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Debug);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void debug_with_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Debug("Hello", new ExternalException());

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Debug);
            Assert.IsInstanceOfType(sut.Entries[0].Exception, typeof(ExternalException));
        }

        [TestMethod]
        public void info_without_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Info("Hello");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Info);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void info_formatted()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Info("Hello {0}", "World");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello World");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Info);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void info_with_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Info("Hello", new ExternalException());

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Info);
            Assert.IsInstanceOfType(sut.Entries[0].Exception, typeof(ExternalException));
        }

        [TestMethod]
        public void warning_without_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Warning("Hello");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Warning);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void warning_formatted()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Warning("Hello {0}", "World");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello World");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Warning);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void warning_with_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Warning("Hello", new ExternalException());

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Warning);
            Assert.IsInstanceOfType(sut.Entries[0].Exception, typeof(ExternalException));
        }

        [TestMethod]
        public void Error_without_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Error("Hello");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Error);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void Error_formatted()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Error("Hello {0}", "World");

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello World");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Error);
            Assert.AreEqual(sut.Entries[0].Exception, null);
        }

        [TestMethod]
        public void Error_with_exception()
        {

            var sut = new BaseLoggerWrapper(GetType());
            sut.Error("Hello", new ExternalException());

            Assert.AreEqual(sut.Entries.Count, 1);
            Assert.AreEqual(sut.Entries[0].Message, "Hello");
            Assert.AreEqual(sut.Entries[0].LogLevel, LogLevel.Error);
            Assert.IsInstanceOfType(sut.Entries[0].Exception, typeof(ExternalException));
        }

        [TestMethod]
        public void format_a_simple_exception()
        {
            try
            {
                var exception = new NotImplementedException();

                var sut = new BaseLoggerWrapper(GetType());
                var actual = sut.FormatException(exception);

                Assert.AreEqual(actual.StartsWith("    System.NotImplementedException"), true);
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
            }
        }

        [TestMethod]
        public void include_property_in_the_exception_output()
        {
            try
            {
                var exception = new ExceptionWithProperty() { UserId = 10 };

                var sut = new BaseLoggerWrapper(GetType());
                var actual = sut.FormatException(exception);

                Assert.AreEqual(actual.StartsWith("    MicroServer.Tests.Logging.Loggers.ExceptionWithProperty"), true);
                Assert.AreEqual(actual.EndsWith("[UserId='10']\r\n"), true);
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
            }
        }

        [TestMethod]
        public void include_properties_in_the_exception_output()
        {
            try {

                var exception = new ExceptionWithProperty2() { UserId = 10, FirstName = "Arne" };

                var sut = new BaseLoggerWrapper(GetType());
                var actual = sut.FormatException(exception);

                Debug.WriteLine(actual.ToString());

                Assert.AreEqual(actual.StartsWith("    MicroServer.Tests.Logging.Loggers.ExceptionWithProperty2"), true);
                Assert.AreEqual(actual.EndsWith("[UserId='10',FirstName='Arne']\r\n"), true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
            }
        }

        [TestMethod]
        public void ignore_null_properties_in_the_exception_output()
        {
            try
            {
                var exception = new ExceptionWithProperty2() { UserId = 10 };

                var sut = new BaseLoggerWrapper(GetType());
                var actual = sut.FormatException(exception);

                Assert.AreEqual(actual.StartsWith("    MicroServer.Tests.Logging.Loggers.ExceptionWithProperty2"), true);
                Assert.AreEqual(actual.EndsWith("[UserId='10']\r\n"), true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
            }
        }

    }

    public class ExternalException : Exception
    {

    }

    public class ExceptionWithProperty : Exception
    {
        public int UserId { get; set; }
    }

    public class ExceptionWithProperty2 : Exception
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
    }
}
