using NUnit.Framework;

namespace FakeItEasy.Core.Tests
{
    [TestFixture]
    public class DummyDefinitionTests
    {
        [Test]
        public void ForType_should_return_the_generic_type_parameter_type()
        {
            var definition = new TestableFakeDefinition();

            Assert.That(definition.ForType, Is.EqualTo(typeof(SomeType)));
        }

        [Test]
        public void CreateFake_should_return_object_from_protected_function()
        {
            var definition = new TestableFakeDefinition() as IDummyDefinition;
            var created = definition.CreateDummy();

            Assert.That(created, Is.InstanceOf<SomeType>());
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
