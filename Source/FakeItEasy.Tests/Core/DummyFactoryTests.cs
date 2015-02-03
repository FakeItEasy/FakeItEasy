namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Globalization;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DummyFactoryTests
    {
        [Test]
        public void Create_with_type_parameter_should_return_object_from_Create()
        {
            var factory = new TestableFakeFactory() as IDummyFactory;
            var created = factory.Create(typeof(SomeType));

            Assert.That(created, Is.InstanceOf<SomeType>());
        }

        [Test]
        public void Create_should_guard_against_bad_type_argument()
        {
            string expectedMessage = string.Format(
                CultureInfo.CurrentCulture,
                "The {0} can only create dummies of type '{1}'.*",
                typeof(TestableFakeFactory),
                typeof(SomeType));

            var factory = new TestableFakeFactory() as IDummyFactory;
            
            var exception = Record.Exception(() => factory.Create(typeof(DummyFactoryTests)));

            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage(expectedMessage)
                .And.ParamName.Should().Be("type");
        }

        public class SomeType
        {
        }

        public class TestableFakeFactory : DummyFactory<SomeType>
        {
            protected override SomeType Create()
            {
                return new SomeType();
            }
        }
    }
}
