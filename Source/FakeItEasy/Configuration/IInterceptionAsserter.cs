namespace FakeItEasy.Configuration
{
    using System.Reflection;

    internal interface IInterceptionAsserter
    {
        void AssertThatMethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget);
    }

    internal class DefaultInterceptionAsserter
        : IInterceptionAsserter
    {
        public void AssertThatMethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget)
        {
        }
    }
}