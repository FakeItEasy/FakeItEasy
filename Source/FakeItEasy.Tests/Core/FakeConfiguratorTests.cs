namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Globalization;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
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
            var expectedMessage = string.Format(
                CultureInfo.CurrentCulture, 
                "The {0} can only configure fakes of type '{1}'.*",
                typeof(TestableConfigurator<IFoo>),
                typeof(IFoo));

            var configurator = new TestableConfigurator<IFoo>() as IFakeConfigurator;

            var exception = Record.Exception(() =>
                configurator.ConfigureFake(string.Empty));
            
            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage(expectedMessage)
                .And.ParamName.Should().Be("fakeObject");
        }

        private class TestableConfigurator<T>
            : FakeConfigurator<T>
        {
            public T InstancePassedToConfigureFake { get; private set; }

            protected override void ConfigureFake(T fakeObject)
            {
                this.InstancePassedToConfigureFake = fakeObject;
            }
        }
    }
}
