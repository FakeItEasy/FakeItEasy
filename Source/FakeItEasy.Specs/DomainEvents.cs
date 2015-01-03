namespace FakeItEasy.Specs
{
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
}