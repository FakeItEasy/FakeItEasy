namespace FakeItEasy.Creation
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the IFakeAndDummyManager interface.
    /// </summary>
    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        private readonly FakeObjectCreator fakeCreator;
        private readonly IDummyValueCreationSession session;
        private readonly IFakeWrapperConfigurer wrapperConfigurer;

        public DefaultFakeAndDummyManager(IDummyValueCreationSession session, FakeObjectCreator fakeCreator, IFakeWrapperConfigurer wrapperConfigurer)
        {
            this.session = session;
            this.fakeCreator = fakeCreator;
            this.wrapperConfigurer = wrapperConfigurer;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            object result;
            if (!this.session.TryResolveDummyValue(typeOfDummy, out result))
            {
                throw new FakeCreationException();
            }

            return result;
        }

        public object CreateFake(Type typeOfFake, FakeOptions options)
        {
            var result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: true);

            this.ApplyConfigurationFromOptions(result, options);

            return result;
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            return this.session.TryResolveDummyValue(typeOfDummy, out result);
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: false);

            if (result == null)
            {
                return false;
            }

            this.ApplyConfigurationFromOptions(result, options);
            return true;
        }

        private void ApplyConfigurationFromOptions(object fake, FakeOptions options)
        {
            if (options.WrappedInstance != null)
            {
                this.wrapperConfigurer.ConfigureFakeToWrap(fake, options.WrappedInstance, options.SelfInitializedFakeRecorder);
            }

            foreach (var a in options.OnFakeCreatedActions)
            {
                a.Invoke(fake);
            }
        }
    }
}