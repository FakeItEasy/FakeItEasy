namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public abstract class DomainEvent
    {
        public static readonly DateTime DefaultTimestamp = new DateTime(2015, 1, 27, 06, 49, 15);

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Required for testing.")]
        protected DomainEvent()
        {
            this.Timestamp = this.CalculateTimestamp();
        }

        public int ID { get; set; }

        public DateTime Timestamp { get; private set; }

        public virtual DateTime CalculateTimestamp()
        {
            return DefaultTimestamp;
        }
    }

    public class RobotActivatedEvent : DomainEvent
    {
    }

    public class RobotRunsAmokEvent : DomainEvent
    {
    }
}