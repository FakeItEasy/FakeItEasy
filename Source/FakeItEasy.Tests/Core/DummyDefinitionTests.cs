namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Globalization;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DummyDefinitionTests
    {
        [Test]
        public void CreateDummyOfType_should_return_object_from_CreateDummy()
        {
            var definition = new TestableFakeDefinition() as IDummyDefinition;
            var created = definition.CreateDummyOfType(typeof(SomeType));

            Assert.That(created, Is.InstanceOf<SomeType>());
        }

        [Test]
        public void CreateDummyOfType_should_guard_against_bad_type_argument()
        {
            string expectedMessage = string.Format(
                CultureInfo.CurrentCulture,
                "The {0} can only create dummies of type '{1}'.*",
                typeof(TestableFakeDefinition),
                typeof(SomeType));

            var definition = new TestableFakeDefinition() as IDummyDefinition;
            
            var exception = Record.Exception(() => definition.CreateDummyOfType(typeof(DummyDefinitionTests)));

            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage(expectedMessage)
                .And.ParamName.Should().Be("type");
        }

        public class SomeType
        {
        }

        public class TestableFakeDefinition : DummyDefinition<SomeType>
        {
            protected override SomeType CreateDummy()
            {
                return new SomeType();
            }
        }
    }
}
