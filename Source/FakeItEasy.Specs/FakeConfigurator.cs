namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class FakeConfigurator
    {
        [Scenario]
        public void when_a_fake_configurator_is_defined_for_a_set_of_types()
        {
            RobotActivatedEvent fake = null;

            "when a fake configurator is defined for a set of types"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "it should configure the fake"
                .x(() => fake.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public void when_two_fake_configurators_apply_to_the_same_type(
            RobotRunsAmokEvent fake)
        {
            "when a fake configurator is defined for a set of types"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "it should use the one with higher priority"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public void when_configuring_a_method_called_by_a_constructor(
            RobotRunsAmokEvent fake)
        {
            "when a fake configurator is defined for a set of types"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "it should use the configured behavior in the constructor"
                .x(() => fake.Timestamp.Should().Be(RobotRunsAmokEventFakeConfigurator.ConfiguredTimestamp));
        }
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