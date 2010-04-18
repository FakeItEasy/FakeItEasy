using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Mef
{
    [TestFixture]
    public class MefContainerTests
    {
        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_definition()
        {
            var container = new MefContainer();

            object fake;

            Assert.That(container.TryCreateFakeObject(typeof(IDefined), out fake), Is.True);
            Assert.That(fake, Is.InstanceOf<Defined>());
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_definition_exists()
        {
            var container = new MefContainer();

            object fake;

            Assert.That(container.TryCreateFakeObject(typeof(IUndefined), out fake), Is.False);
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_for_registered_configuration()
        {
            var container = new MefContainer();

            var fake = A.Fake<IUndefined>();

            container.ConfigureFake(typeof(IUndefined), fake);

            Assert.That(fake.WasConfigured(), Is.True);
        }

        [Test]
        public void ConfigureFake_should_do_nothing_when_fake_type_has_no_configuration_specified()
        {
            var container = new MefContainer();

            var fake = A.Fake<IUndefined>();
        }
    }

    public class ConfigurationForIDefined : FakeConfigurator<IUndefined>
    {
        public override void ConfigureFake(IUndefined fakeObject)
        {
            A.CallTo(() => fakeObject.WasConfigured()).Returns(true);
        }
    }


    public interface IUndefined
    {
        bool WasConfigured();
    }

    public interface IDefined
    {
        void Bar();
    }

    public class IDefinedDefinition
        : FakeDefinition<IDefined>
    {
        protected override IDefined CreateFake()
        {
            return new Defined();
        }
    }

    public class Defined
        : IDefined
    {
        public void Bar()
        {
            throw new NotImplementedException();
        }

        public bool WasConfigured;
    }

}