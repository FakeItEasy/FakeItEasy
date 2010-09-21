using NUnit.Framework;
using System;
using FakeItEasy.Core;
using System.Collections.Generic;
using System.Linq;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class FakingClassesTests
    {
        [Test]
        public void Should_be_able_to_get_a_dummy_value_of_uri_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                var result = A.Fake<Uri>();
            }
        }
    }
}
