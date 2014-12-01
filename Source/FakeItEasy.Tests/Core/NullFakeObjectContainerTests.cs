namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class NullFakeObjectContainerTests
    {
        [Test]
        public void TryCreateFakeObject_should_return_false()
        {
            var container = new NullFakeObjectContainer();

            object result = null;
            Assert.That(container.TryCreateDummyObject((Type)null, out result), Is.False);
        }

        [Test]
        public void TryCreateFakeObject_should_set_output_variable_to_null()
        {
            var container = new NullFakeObjectContainer();
            object result = null;

            container.TryCreateDummyObject((Type)null, out result);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ConfigureFakeObject_should_do_nothing()
        {
            var fake = A.Fake<IFoo>();

            A.CallTo(fake).Throws(new InvalidOperationException());

            var container = new NullFakeObjectContainer();

            container.ConfigureFake(typeof(IFoo), fake);
        }
    }
}
