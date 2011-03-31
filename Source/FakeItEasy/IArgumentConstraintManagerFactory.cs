namespace FakeItEasy
{
    internal interface IArgumentConstraintManagerFactory
    {
        IArgumentConstraintManager<T> Create<T>();
    }
}