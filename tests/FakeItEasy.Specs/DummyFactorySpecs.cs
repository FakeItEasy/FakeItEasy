namespace FakeItEasy.Specs
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Xbehave;

    public static class DummyFactorySpecs
    {
        [Scenario]
        public static void DummyFactoryUsage(
            RobotActivatedEvent dummy)
        {
            "When I create a Dummy of a type that has a dummy factory defined"
                .x(() => dummy = A.Dummy<RobotActivatedEvent>());

            "Then it should be created by the factory"
                .x(() => dummy.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public static void DummyFactoryPriority(
            RobotRunsAmokEvent dummy)
        {
            "Given two dummy factories which apply to the same type"
                .x(() => { }); // see DomainEventDummyFactory and RobotRunsAmokEventDummyFactory

            "When I create a Dummy of the type"
                .x(() => dummy = A.Dummy<RobotRunsAmokEvent>());

            "Then it should be created by the factory with the higher priority"
                .x(() => dummy.ID.Should().Be(-17));
        }

        [Scenario]
        public static void GenericDummyFactoryDefaultPriority(
            IDummyFactory formatter,
            Priority priority)
        {
            "Given an argument value formatter that does not override priority"
                .x(() => formatter = new SomeDummyFactory());

            "When I fetch the Priority"
                .x(() => priority = formatter.Priority);

            "Then it should be the default priority"
                .x(() => priority.Should().Be(Priority.Default));
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

        public Priority Priority => Priority.Default;

        public bool CanCreate(Type type)
        {
            return typeof(DomainEvent).GetTypeInfo().IsAssignableFrom(type);
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
        public override Priority Priority => new Priority(3);

        protected override RobotRunsAmokEvent Create()
        {
            return new RobotRunsAmokEvent { ID = -17 };
        }
    }
}
