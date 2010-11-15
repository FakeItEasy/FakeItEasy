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

            NextCall.To(fake).WithAnyArguments().Throws(new ApplicationException());
            fake.Baz(null, null);

            Assert.Throws<ApplicationException>(() =>
                fake.Baz("foo", "bar"));
        }
    }
}
