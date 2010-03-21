using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;

namespace FakeItEasy.Tests.Core
{
    [TestFixture]
    public class NullFakeObjectContainerTests
    {
        [Test]
        public void TryCreateFakeObject_should_return_false()
        {
            var container = new NullFakeObjectContainer();

            object result = null;
            Assert.That(container.TryCreateFakeObject((Type)null, out result), Is.False);
        }

        [Test]
        public void TryCreateFakeObject_should_set_output_variable_to_null()
        {
            var container = new NullFakeObjectContainer();
            object result = null;

            container.TryCreateFakeObject((Type)null, out result);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ConfigureFakeObject_should_do_nothing()
        {
            var fake = A.Fake<IFoo>();
            Any.CallTo(fake).Throws(new Exception());

            var container = new NullFakeObjectContainer();

            container.ConfigureFake(typeof(IFoo), fake);
        }
    }
}
