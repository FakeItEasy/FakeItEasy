namespace FakeItEasy
{
    public interface IRepeatSpecification
    {
        Repeated Once { get; }

        Repeated Twice { get; }

        Repeated Times(int numberOfTimes);
    }
}