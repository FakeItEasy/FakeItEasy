namespace FakeItEasy.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class AnyTests
        : ConfigurableServiceLocatorTestBase
    {
#pragma warning disable 618
        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Any.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = string.Empty;

            Assert.That(Any.ReferenceEquals(s, s), Is.True);
        }
#pragma warning restore 618

        protected override void OnSetup()
        {
        }
    }
}
