namespace FakeItEasy.Tests.Configuration
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VisualBasicHelpersTests
    {
        [Test]
        public void With_any_arguments_should_configure_call_so_that_any_arguments_matches()
        {
            var fake = A.Fake<IFoo>();

            NextCall.To(fake).WithAnyArguments().Throws(new InvalidOperationException());
            fake.Baz(null, null);

            Assert.Throws<InvalidOperationException>(() => fake.Baz("foo", "bar"));
        }
    }
}
