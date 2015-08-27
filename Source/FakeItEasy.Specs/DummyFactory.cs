namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class DummyFactory
    {
        [Scenario]
        public void when_a_dummy_factory_is_defined_for_a_set_of_types(RobotActivatedEvent dummy)
        {
            "when a dummy factory is defined for a set of types"
                .x(() => dummy = A.Dummy<RobotActivatedEvent>());

            "it should create a dummy from the factory"
                .x(() => dummy.ID.Should().BeGreaterThan(0));
        }

        [Scenario]
        public void when_two_dummy_factories_apply_to_the_same_type(RobotRunsAmokEvent dummy)
        {
            "when two dummy factories apply to the same type"
                .x(() => dummy = A.Dummy<RobotRunsAmokEvent>());

            "it should use the one with higher priority"
                .x(() => dummy.ID.Should().Be(-17));
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