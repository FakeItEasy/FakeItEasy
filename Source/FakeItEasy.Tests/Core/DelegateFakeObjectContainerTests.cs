namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class DelegateFakeObjectContainerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TryCreateFakeObject_should_return_object_from_registered_delegate()
        {
            var container = this.CreateContainer();

            var foo = A.Fake<IFoo>();

            container.Register<IFoo>(() => foo);

            object result;
            Assert.That(container.TryCreateDummyObject(typeof(IFoo), out result), Is.True);
            Assert.That(result, Is.SameAs(foo));
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_delegate_is_registered_for_the_requested_type()
        {
            var container = this.CreateContainer();

            object result;
            Assert.That(container.TryCreateDummyObject(typeof(IFoo), out result), Is.False);
        }

        [Test]
        public void Register_should_be_able_to_register_the_same_type_twice_to_override_old_registration()
        {
            var container = this.CreateContainer();

            var foo1 = A.Fake<IFoo>();
            var foo2 = A.Fake<IFoo>();

            container.Register<IFoo>(() => foo1);
            container.Register<IFoo>(() => foo2);

            object result;
            container.TryCreateDummyObject(typeof(IFoo), out result);

            Assert.That(result, Is.SameAs(foo2));
        }

        [Test]
        public void ConfigureFakeObject_should_do_nothing()
        {
            var fake = A.Fake<IFoo>();

            A.CallTo(fake).Throws(new InvalidOperationException());

            var container = this.CreateContainer();

            container.ConfigureFake(typeof(IFoo), fake);
        }

        private DelegateFakeObjectContainer CreateContainer()
        {
            return new DelegateFakeObjectContainer();
        }
    }
}
