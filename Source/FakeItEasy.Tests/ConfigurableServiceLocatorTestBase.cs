namespace FakeItEasy.Tests
{
    using NUnit.Framework;
    
    public abstract class ConfigurableServiceLocatorTestBase
    {
        private ServiceLocator replacedServiceLocator;
        
        [SetUp]
        public void Setup()
        {            
            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(x => x.Wrapping(ServiceLocator.Current));

            this.OnSetup();
        }

        [TearDown]
        public void Teardown()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
            this.OnTeardown();
        }

        protected virtual void OnSetup()
        {
        }

        protected virtual void OnTeardown()
        {
        }

        protected void StubResolve<T>(T returnedInstance)
        {
            A.CallTo(() => ServiceLocator.Current.Resolve(typeof(T))).Returns(returnedInstance);
        }

        protected T StubResolveWithFake<T>()
        {
            var result = A.Fake<T>();

            this.StubResolve<T>(result);

            return result;
        }
    }
}
