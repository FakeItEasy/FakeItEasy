namespace FakeItEasy.Core
{
    internal interface IArgumentConstraintManagerFactory
    {
        IArgumentConstraintManager<T> Create<T>();
    }
}