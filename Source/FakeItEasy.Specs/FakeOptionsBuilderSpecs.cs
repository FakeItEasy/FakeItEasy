namespace FakeItEasy.Specs
{
    using System;
    using Creation;
    using FluentAssertions;
    using Xbehave;

    public class FakeOptionsBuilderSpecs
    {
        [Scenario]
        public void DefinedFakeOptionsBuilder(
            RobotActivatedEvent fake)
        {
            "When a fake is created for a type that has an options builder defined"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "Then the builder will apply the fake options builder"
                .x(() => fake.ID.Should().BeGreaterThan(0));

            "And it will be passed the fake type"
                .x(() => fake.Name.Should().Be(typeof(RobotActivatedEvent).Name));
        }

        [Scenario]
        public void FakeOptionsBuilderPriority(
            RobotRunsAmokEvent fake)
        {
            "When a fake is created for a type that has two applicable fake options builders"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the builder with the higher priority will build options for the fake"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public void DuringConstruction(
            RobotRunsAmokEvent fake)
        {
            "When a fake is created for a type that has an options builder defined"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the builder will build options to be used during the fake's constructor"
                .x(() => fake.Timestamp.Should().Be(RobotRunsAmokEventFakeOptionsBuilder.ConfiguredTimestamp));
        }
    }

    public class DomainEventFakeOptionsBuilder : IFakeOptionsBuilder
    {
        private int nextID = 1;

        public int Priority
        {
            get { return -1; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return typeof(DomainEvent).IsAssignableFrom(type);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options == null)
            {
                return;
            }

            options.ConfigureFake(fake =>
            {
                var domainEvent = (DomainEvent)fake;
                domainEvent.ID = this.nextID++;
                domainEvent.Name = typeOfFake.Name;
            });
        }
    }

    public class RobotRunsAmokEventFakeOptionsBuilder : IFakeOptionsBuilder
    {
        public static readonly DateTime ConfiguredTimestamp = new DateTime(1997, 8, 29, 2, 14, 03);

        public int Priority
        {
            get { return 0; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(RobotRunsAmokEvent);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options == null)
            {
                return;
            }

            options.ConfigureFake(fake =>
            {
                var robotRunsAmokEvent = (RobotRunsAmokEvent)fake;
                A.CallTo(() => robotRunsAmokEvent.CalculateTimestamp())
                    .Returns(ConfiguredTimestamp);
                robotRunsAmokEvent.ID = -99;
            });
        }
    }
}