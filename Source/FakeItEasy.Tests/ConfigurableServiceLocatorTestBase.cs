namespace FakeItEasy.Tests
{
    using NUnit.Framework;
    
    public abstract class ConfigurableServiceLocatorTestBase
    {
        private ServiceLocator replacedServiceLocator;
        
        [SetUp]
        public void SetUp()
        {
            
            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(x => x.Wrapping(ServiceLocator.Current));

            this.OnSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
            this.OnTearDown();
        }

        protected virtual void OnSetUp()
        {
        }

        protected virtual void OnTearDown()
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
