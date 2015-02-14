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

    public class when_configuring_a_method_called_by_a_constructor
    {
        private static RobotRunsAmokEvent fake = null;

        private Because of = () => { fake = A.Fake<RobotRunsAmokEvent>(); };

        private It should_use_the_configured_behavior_in_the_constructor =
            () => fake.Timestamp.Should().Be(RobotRunsAmokEventFakeConfigurator.ConfiguredTimestamp);
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
        public static readonly DateTime ConfiguredTimestamp = new DateTime(1997, 8, 29, 2, 14, 03);

        protected override void ConfigureFake(RobotRunsAmokEvent fakeObject)
        {
            if (fakeObject != null)
            {
                A.CallTo(() => fakeObject.CalculateTimestamp()).Returns(ConfiguredTimestamp);
                fakeObject.ID = -99;
            }
        }
    }
}