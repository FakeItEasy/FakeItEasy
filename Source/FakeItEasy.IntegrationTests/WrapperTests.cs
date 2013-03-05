namespace FakeItEasy.IntegrationTests
{
    using System.Collections.Generic;
    using System.IO;
    using FakeItEasy.ExtensionSyntax;
    using NUnit.Framework;

    [TestFixture]
    public class WrapperTests
    {
        [Test]
        public void Wrapper_should_only_delegate_non_configured_calls()
        {
            var stream = new MemoryStream();
            var wrapper = A.Fake<Stream>(x => x.Wrapping(stream));

            Assert.IsTrue(wrapper.CanRead);

            wrapper.Configure().CallsTo(x => x.CanRead).Returns(false);

            Assert.That(wrapper.CanRead, Is.False);
            Assert.That(stream.CanRead, Is.True);
            Assert.That(this.CanRead(wrapper), Is.False);
        }

        [Test]
        public void Wrapper_should_pass_values_to_wrapped_instance()
        {
            var dictionary = new Dictionary<string, string>();
            var fake = A.Fake<IDictionary<string, string>>(x => x.Wrapping(dictionary));

            fake.Add("foo", "bar");

            Assert.That(fake["foo"], Is.EqualTo("bar"));
            A.CallTo(() => fake.Add("foo", "bar")).MustHaveHappened();
        }

        private bool CanRead(Stream stream)
        {
            return stream.CanRead;
        }
    }
}
