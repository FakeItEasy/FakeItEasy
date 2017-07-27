namespace FakeItEasy
{
    internal interface IArgumentConstraintManagerFactory
    {
        INegatableArgumentConstraintManager<T> Create<T>();
    }
}
