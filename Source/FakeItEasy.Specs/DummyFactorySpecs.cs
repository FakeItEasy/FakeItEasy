namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public static class DummyFactorySpecs
    {
        [Scenario]
        public static void DummyFactoryUsage(
            RobotActivatedEvent dummy)
        {
            "when a dummy factory is defined for a set of types"
                .x(() => dummy = A.Dummy<RobotActivatedEvent>());

            "it should create a dummy from the factory"
                .x(() => dummy.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public static void DummyFactoryPriority(
            RobotRunsAmokEvent dummy)
        {
            "when two dummy factories apply to the same type"
                .x(() => dummy = A.Dummy<RobotRunsAmokEvent>());

            "it should use the one with higher priority"
                .x(() => dummy.ID.Should().Be(-17));
        }

        [Scenario]
        public static void GenericDummyFactoryDefaultPriority(
            IDummyFactory formatter,
            int priority)
        {
            "Given an argument value formatter that does not override priority"
                .x(() => formatter = new SomeDummyFactory());

            "When I fetch the Priority"
                .x(() => priority = formatter.Priority);

            "Then it should be 0"
                .x(() => priority.Should().Be(0));
        }

        private class SomeClass
        {
        }

        private class SomeDummyFactory : DummyFactory<SomeClass>
        {
            protected override SomeClass Create()
            {
                return new SomeClass();
            }
        }
    }

    public class DomainEventDummyFactory : IDummyFactory
    {
        private int nextID = 1;

        public int Priority
        {
            get { return -3; }
        }

        public bool CanCreate(Type type)
        {
            return typeof(DomainEvent).IsAssignableFrom(type);
        }

        public object Create(Type type)
        {
            var dummy = (DomainEvent)Activator.CreateInstance(type);
            dummy.ID = this.nextID++;
            return dummy;
        }
    }

    public class RobotRunsAmokEventDummyFactory : DummyFactory<RobotRunsAmokEvent>
    {
        protected override RobotRunsAmokEvent Create()
        {
            return new RobotRunsAmokEvent { ID = -17 };
        }
    }
}
