namespace FakeItEasy.IntegrationTests
{
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class WrapperTests
    {
        [Test]
        public void Wrapper_should_only_delegate_non_configured_calls()
        {
            var stream = new MemoryStream();
            var wrapper = A.Fake<Stream>(x => x.Wrapping(stream));

            wrapper.CanRead.Should().BeTrue();

            A.CallTo(() => wrapper.CanRead).Returns(false);

            wrapper.CanRead.Should().BeFalse();
            stream.CanRead.Should().BeTrue();
            this.CanRead(wrapper).Should().BeFalse();
        }

        [Test]
        public void Wrapper_should_pass_values_to_wrapped_instance()
        {
            var dictionary = new Dictionary<string, string>();
            var fake = A.Fake<IDictionary<string, string>>(x => x.Wrapping(dictionary));

            fake.Add("foo", "bar");

            fake["foo"].Should().Be("bar");
            A.CallTo(() => fake.Add("foo", "bar")).MustHaveHappened();
        }

        private bool CanRead(Stream stream)
        {
            return stream.CanRead;
        }
    }
}
