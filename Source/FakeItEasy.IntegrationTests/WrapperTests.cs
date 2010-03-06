using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;
using System.IO;
using System.Web.UI;
using FakeItEasy.Core;

namespace FakeItEasy.IntegrationTests
{
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
            Assert.That(CanRead(wrapper), Is.False);
        }

        [Test]
        public void Wrapper_should_pass_values_to_wrapped_instance()
        {
            var dictionary = new Dictionary<string, string>();
            var fake = A.Fake<IDictionary<string, string>>(x => x.Wrapping(dictionary));

            fake.Add("foo", "bar");

            Assert.That(fake["foo"], Is.EqualTo("bar"));
            Fake.Assert(fake)
                .WasCalled(x => x.Add("foo", "bar"));


        }

        private bool CanRead(Stream stream)
        {
            return stream.CanRead;
        }
    }
}
