using System;

using MicroServer.Logging.Providers;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MicroServer.Test.Logging
{
    [TestClass]
    public class NamespaceFilterTests
    {
        [TestMethod]
        public void child_ns_is_configured_to_be_allowed__so_allow_it()
        {
            var sut = new NamespaceFilter();
            sut.Allow("MicroServer", true);
            var actual = sut.IsSatisfiedBy(typeof(NoFilter));

            Assert.AreEqual(actual, true);
        }

        [TestMethod]
        public void child_ns_is_configured_to_NOT_be_allowed__so_dont_allow_it()
        {
            var sut = new NamespaceFilter();
            sut.Allow("MicroServer", false);
            var actual = sut.IsSatisfiedBy(typeof(NoFilter));

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void no_filters_means_that_everything_is_revoked()
        {
            var sut = new NamespaceFilter();
            var actual = sut.IsSatisfiedBy(typeof(NoFilter));

            Assert.AreEqual(actual, false);
        }

        [TestMethod]
        public void allow_root_but_reject_specific_child__make_sure_that_the_root_is_allowed()
        {
            var sut = new NamespaceFilter();
            sut.Revoke(typeof(NoFilter).Namespace, false);
            sut.Allow("MicroServer", true);
            var actual = sut.IsSatisfiedBy(typeof(Guid));

            Assert.AreEqual(actual, true);
        }

        [TestMethod]
        public void allow_root_but_reject_specific_child()
        {
            var sut = new NamespaceFilter();
            sut.Revoke(typeof (NoFilter).Namespace, false);
            sut.Allow("MicroServer", true);
            var actual = sut.IsSatisfiedBy(typeof(NoFilter));

            Assert.AreEqual(actual, false, "because a specific filter was set");
        }
    }
}