using FakeItEasy.Configuration;
using FakeItEasy.Core;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
	[TestFixture]
    public class AnyTests
        : ConfigurableServiceLocatorTestBase
    {
        protected override void OnSetUp()
        {

        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Any.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(Any.ReferenceEquals(s, s), Is.True);
        }
    }
}
