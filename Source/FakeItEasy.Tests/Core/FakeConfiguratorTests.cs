using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.Core
{
    [TestFixture]
    public class FakeConfiguratorTests
    {
        [Test]
        public void ForType_should_return_type_of_type_parameter()
        {
            var configurator = new TestableConfigurator<IFoo>();

            Assert.That(configurator.ForType, Is.EqualTo(typeof(IFoo)));
        }

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
                configurator.ConfigureFake(""));
            Assert.That(thrown.Message, Text.StartsWith("The FakeItEasy.Core.FakeConfiguratorTests+TestableConfigurator`1[FakeItEasy.Tests.IFoo] can only configure fakes of the type 'FakeItEasy.Tests.IFoo'."));
            Assert.That(thrown.ParamName, Is.EqualTo("fakeObject"));
        }

        private class TestableConfigurator<T>
            : FakeConfigurator<T>
        {
            public T InstancePassedToConfigureFake;

            public override void ConfigureFake(T fakeObject)
            {
                this.InstancePassedToConfigureFake = fakeObject;
            }
        }

    }
}
