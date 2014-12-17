namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class FakeConfiguratorTests
    {
        [Test]
        public void ConfigureFake_should_call_abstract_method()
        {
            var configurator = new TestableConfigurator<IFoo>();

            var foo = A.Fake<IFoo>();

            ((IFakeConfigurator)configurator).ConfigureFake(foo);

            Assert.That(configurator.InstancePassedToConfigureFake, Is.SameAs(foo));
        }

        [Test]
        public void ConfigureFake_should_be_null_guarded()
        {
            var configurator = new TestableConfigurator<IFoo>() as IFakeConfigurator;

            NullGuardedConstraint.Assert(() =>
                configurator.ConfigureFake(A.Fake<IFoo>()));
        }

        [Test]
        [SetCulture("en-US")]
        public void ConfigureFake_should_throw_when_the_specified_fake_object_is_not_of_the_correct_type()
        {
            var configurator = new TestableConfigurator<IFoo>() as IFakeConfigurator;

            var thrown = Assert.Throws<ArgumentException>(() =>
                configurator.ConfigureFake(string.Empty));
            Assert.That(thrown.Message, Is.StringStarting("The FakeItEasy.Core.FakeConfiguratorTests+TestableConfigurator`1[FakeItEasy.Tests.IFoo] can only configure fakes of type 'FakeItEasy.Tests.IFoo'."));
            Assert.That(thrown.ParamName, Is.EqualTo("fakeObject"));
        }

        private class TestableConfigurator<T>
            : FakeConfigurator<T>
        {
            public T InstancePassedToConfigureFake { get; set; }

            public override void ConfigureFake(T fakeObject)
            {
                this.InstancePassedToConfigureFake = fakeObject;
            }
        }
    }
}
