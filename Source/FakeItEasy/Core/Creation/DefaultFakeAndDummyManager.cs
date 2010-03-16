namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
            if (!this.container.TryCreateFakeObject(typeOfDummy, out result))
            {
                result = this.CreateProxy(typeOfDummy, null, false);
            }

            return result != null;
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            throw new NotImplementedException();
        }

        internal virtual object CreateProxy(Type typeOfProxy, IEnumerable<object> argumentsForConstructor, bool throwOnFailure)
        {
            var fakeObject = this.CreateNewFakeObject();
            
            var proxyResult = this.proxyGenerator.GenerateProxy(typeOfProxy, fakeObject, argumentsForConstructor);

            if (throwOnFailure)
            {
                AssertThatProxyWasSuccessfullyCreated(typeOfProxy, proxyResult);
            }

            fakeObject.SetProxy(proxyResult);

            return proxyResult.Proxy;
        }

        private static void AssertThatProxyWasSuccessfullyCreated(Type typeOfProxy, ProxyResult proxyResult)
        {
            if (!proxyResult.ProxyWasSuccessfullyCreated)
            {
                var message = CreateFailedToGenerateProxyErrorMessage(typeOfProxy, proxyResult);
                throw new FakeCreationException(message);
            }
        }

        private static string CreateFailedToGenerateProxyErrorMessage(Type typeOfProxy, ProxyResult proxyResult)
        {
            var messageFromProxyGenerator = Indent(proxyResult.ErrorMessage);
            
            var writer = new StringWriter();

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(CommonExtensions.FormatInvariant(ExceptionMessages.FailedToGenerateProxyPattern, typeOfProxy, messageFromProxyGenerator));
            writer.WriteLine();

            return Indent(writer.GetStringBuilder().ToString());
        }

        private static string Indent(string value)
        { 
            var lines = value.Split(new [] {Environment.NewLine}, StringSplitOptions.None);

            return string.Join(Environment.NewLine,
                (from line in lines
                 select line.Length == 0 ? string.Empty : "  " + line).ToArray());

        }

        private FakeObject CreateNewFakeObject()
        {
            return this.fakeObjectFactory.Invoke();
        }
    }
}
