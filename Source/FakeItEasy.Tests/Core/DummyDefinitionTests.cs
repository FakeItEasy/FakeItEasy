namespace FakeItEasy.Core.Tests
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
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
            string expectedMessage = "The FakeItEasy.Core.Tests.DummyDefinitionTests+TestableFakeDefinition can only create dummies of type 'FakeItEasy.Core.Tests.DummyDefinitionTests+SomeType'.";
            var definition = new TestableFakeDefinition() as IDummyDefinition;
            
            var exception = Record.Exception(() => definition.CreateDummyOfType(typeof(DummyDefinitionTests)));

            Assert.That(exception, Is.InstanceOf<ArgumentException>());
            Assert.That(exception.Message, Is.StringStarting(expectedMessage));
            Assert.That(((ArgumentException)exception).ParamName, Is.EqualTo("type"));
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
