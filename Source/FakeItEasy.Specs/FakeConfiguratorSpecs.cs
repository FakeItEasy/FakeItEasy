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

            "and it should be configured before its constructor is run"
                .x(() => fake.Timestamp.Should().Be(DomainEventFakeConfigurator.ConfiguredTimestamp));
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
    }

    public class DomainEventFakeConfigurator : IFakeConfigurator
    {
        public static readonly DateTime ConfiguredTimestamp = new DateTime(1997, 8, 29, 2, 14, 03);

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
            var domainEvent = (DomainEvent)fakeObject;
            domainEvent.ID = this.nextID++;
            A.CallTo(() => domainEvent.CalculateTimestamp()).Returns(ConfiguredTimestamp);
        }
    }

    public class RobotRunsAmokEventFakeConfigurator : FakeConfigurator<RobotRunsAmokEvent>
    {
        protected override void ConfigureFake(RobotRunsAmokEvent fakeObject)
        {
            if (fakeObject != null)
            {
                fakeObject.ID = -99;
            }
        }
    }
}