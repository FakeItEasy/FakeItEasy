namespace FakeItEasy
{
    internal interface IArgumentConstraintManagerFactory
    {
        ICapturableArgumentConstraintManager<T> Create<T>();
    }
}
