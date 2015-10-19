namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public static class FakeConfiguratorSpecs
    {
        [Scenario]
        public static void DefinedFakeConfigurator(
            RobotActivatedEvent fake)
        {
            "when creating a fake that has a matching configurator"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "then it should be configured"
                .x(() => fake.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public static void FakeConfiguratorPriority(
            RobotRunsAmokEvent fake)
        {
            "when creating a fake that has two matching configurators"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "then it should be configured by the one with higher priority"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public static void DuringConstruction(
            RobotRunsAmokEvent fake)
        {
            "when creating a fake that has a matching configurator"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "then it should be configured before its constructor is run"
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