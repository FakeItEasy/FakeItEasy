namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class FakeConfiguratorSpecs
    {
        [Scenario]
        public void DefinedFakeConfigurator()
        {
            RobotActivatedEvent fake = null;

            "when a fake configurator is defined for a set of types"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "it should configure the fake"
                .x(() => fake.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public void FakeConfiguratorPriority(
            RobotRunsAmokEvent fake)
        {
            "when two fake configurators apply to the same type"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "it should use the one with higher priority"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public void DuringConstruction(
            RobotRunsAmokEvent fake)
        {
            "when configuring a method called by a constructor"
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