namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_a_dummy_factory_is_defined_for_a_set_of_types
    {
        private static RobotActivatedEvent dummy = null;

        private Because of = () => { dummy = A.Dummy<RobotActivatedEvent>(); };

        private It should_create_a_dummy_from_the_factory = () => dummy.ID.Should().BeGreaterThan(0);
    }

    public class when_two_dummy_factories_apply_to_the_same_type
    {
        private static RobotRunsAmokEvent dummy = null;

        private Because of = () => { dummy = A.Dummy<RobotRunsAmokEvent>(); };

        private It should_use_the_one_with_higher_priority =
            () => dummy.ID.Should().Be(-17);
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