namespace FakeItEasy.Specs
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_a_dummy_definition_is_defined_for_a_set_of_types
    {
        private static RobotActivatedEvent foundDummy = null;

        private Because of = () => { foundDummy = A.Dummy<RobotActivatedEvent>(); };

        private It should_create_a_dummy_from_the_definition = () => foundDummy.ID.Should().BeGreaterThan(0);
    }

    public class when_two_dummy_definitions_apply_to_the_same_type
    {
        private static RobotRunsAmokEvent foundDummy = null;

        private Because of = () => { foundDummy = A.Dummy<RobotRunsAmokEvent>(); };

        private It should_use_the_one_with_higher_priority =
            () => foundDummy.ID.Should().Be(-17);
    }

    public abstract class DomainEvent
    {
        public int ID { get; set; }
    }

    public class RobotActivatedEvent : DomainEvent
    {
    }

    public class RobotRunsAmokEvent : DomainEvent
    {
    }

    public class DomainEventDummyDefinition : IDummyDefinition
    {
        private int nextID = 1;

        public int Priority
        {
            get { return 0; }
        }

        public bool CanCreateDummyOfType(Type type)
        {
            return typeof(DomainEvent).IsAssignableFrom(type);
        }

        public object CreateDummyOfType(Type type)
        {
            var dummy = (DomainEvent)Activator.CreateInstance(type);
            dummy.ID = this.nextID++;
            return dummy;
        }
    }

    public class RobotRunsAmokEventDummyDefinition : DummyDefinition<RobotRunsAmokEvent>
    {
        public override int Priority
        {
            get { return 3; }
        }

        protected override RobotRunsAmokEvent CreateDummy()
        {
            return new RobotRunsAmokEvent { ID = -17 };
        }
    }
}