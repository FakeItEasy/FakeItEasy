using NUnit.Framework;
using FakeItEasy.Tests;
using System;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class AssertionsTests
    {
        [Test]
        public void Method_that_is_configured_to_throw_should_still_be_recorded()
        {
            var fake = A.Fake<IFoo>();
            
            A.CallTo(() => fake.Bar()).Throws(new Exception()).Once();

            try
            {
                fake.Bar();
            }
            catch { }

            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }
    }
}
