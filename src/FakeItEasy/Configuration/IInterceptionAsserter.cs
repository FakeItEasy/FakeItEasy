namespace FakeItEasy.Configuration
{
    using System.Reflection;

    internal interface IInterceptionAsserter
    {
        void AssertThatMethodCanBeInterceptedOnInstance(MethodInfo method, object? callTarget);
    }
}
