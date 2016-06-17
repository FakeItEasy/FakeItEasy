namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public abstract class ConfigurableServiceLocatorTestBase : IDisposable
    {
        private readonly ServiceLocator replacedServiceLocator;

        protected ConfigurableServiceLocatorTestBase()
        {
            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(x => x.Wrapping(ServiceLocator.Current));
        }

        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly",
            Justification = "Children do not have a finalizer.")]
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
            Justification = "Implements the Disposable method only to support xUnit test lifecycle.")]
        public void Dispose()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
        }

        protected void StubResolve<T>(T returnedInstance)
        {
            A.CallTo(() => ServiceLocator.Current.Resolve(typeof(T))).Returns(returnedInstance);
        }

        protected T StubResolveWithFake<T>()
        {
            var result = A.Fake<T>();

            this.StubResolve(result);

            return result;
        }
    }
}
