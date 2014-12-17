namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_a_fake_configurator_is_defined_for_a_set_of_types
    {
        private static RobotActivatedEvent fake = null;

        private Because of = () => { fake = A.Fake<RobotActivatedEvent>(); };

        private It should_configure_the_fake = () => fake.ID.Should().BeGreaterThan(0);
    }

    public class when_two_fake_configurators_apply_to_the_same_type
    {
        private static RobotRunsAmokEvent fake = null;

        private Because of = () => { fake = A.Fake<RobotRunsAmokEvent>(); };

        private It should_use_the_one_with_higher_priority = () => fake.ID.Should().Be(-99);
    }

    public class DomainEventFakeConfigurator : IFakeConfigurator
    {
        private int nextID = 1;

        public int Priority
        {
            get { return -1; }
        }

        public bool CanConfigureFakeOfType(Type type)
        {
            return typeof(DomainEvent).IsAssignableFrom(type);
        }

        public void ConfigureFake(object fakeObject)
        {
            ((DomainEvent)fakeObject).ID = this.nextID++;
        }
    }

    public class RobotRunsAmokEventFakeConfigurator : FakeConfigurator<RobotRunsAmokEvent>
    {
        public override void ConfigureFake(RobotRunsAmokEvent fakeObject)
        {
            if (fakeObject != null)
            {
                fakeObject.ID = -99;
            }
        }
    }
}