namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;

    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        private IFakeObjectContainer container;
        private IProxyGeneratorNew proxyGenerator;
        private FakeObject.Factory fakeObjectFactory;

        public DefaultFakeAndDummyManager(IFakeObjectContainer container, IProxyGeneratorNew proxyGenerator, FakeObject.Factory fakeObjectFactory)
        {
            this.container = container;
            this.proxyGenerator = proxyGenerator;
            this.fakeObjectFactory = fakeObjectFactory;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            object result = null;
            
            if (!this.container.TryCreateFakeObject(typeOfDummy, out result))
            {
                return this.CreateProxy(typeOfDummy, null, true);
            }

            return result;
        }

        public object CreateFake(Type typeOfFake, FakeOptions options)
        {
            return this.CreateProxy(typeOfFake, options.ArgumentsForConstructor, true);
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            throw new NotImplementedException();
        }

        internal virtual object CreateProxy(Type typeOfDummy, IEnumerable<object> argumentsForConstructor, bool throwOnFailure)
        {
            var fakeObject = this.CreateNewFakeObject();
            
            var proxyResult = this.proxyGenerator.GenerateProxy(typeOfDummy, fakeObject, argumentsForConstructor);

            if (throwOnFailure)
            {
                AssertThatProxyWasSuccessfullyCreated(typeOfDummy, proxyResult);
            }

            fakeObject.SetProxy(proxyResult);

            return proxyResult.Proxy;
        }

        private static void AssertThatProxyWasSuccessfullyCreated(Type typeOfDummy, ProxyResult proxyResult)
        {
            if (!proxyResult.ProxyWasSuccessfullyCreated)
            {
                throw new FakeCreationException(CommonExtensions.FormatInvariant(ExceptionMessages.FailedToGenerateProxyPattern, typeOfDummy, IndentErrorMessage(proxyResult)));
            }
        }

        private static string IndentErrorMessage(ProxyResult proxyResult)
        {
            return proxyResult.ErrorMessage.Replace(Environment.NewLine, Environment.NewLine + "    ");
        }

        private FakeObject CreateNewFakeObject()
        {
            return this.fakeObjectFactory.Invoke();
        }
    }
}
