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
        private static readonly Logger logger = Log.GetLogger<DefaultFakeAndDummyManager>();

        private readonly FakeObjectCreator fakeCreator;
        private readonly IDummyValueCreationSession session;
        private readonly IFakeWrapperConfigurer wrapperConfigurer;

        public DefaultFakeAndDummyManager(IDummyValueCreationSession session, FakeObjectCreator fakeCreator, IFakeWrapperConfigurer wrapperConfigurer)
        {
            logger.Debug("Created new instance.");

            this.session = session;
            this.fakeCreator = fakeCreator;
            this.wrapperConfigurer = wrapperConfigurer;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            logger.Debug("Creating dummy.");

            object result;
            if (!this.session.TryResolveDummyValue(typeOfDummy, out result))
            {
                throw new FakeCreationException();
            }

            return result;
        }

        public object CreateFake(Type typeOfFake, FakeOptions options)
        {
            logger.Debug("Creating fake.");

            var result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: true);

            if (options.WrappedInstance != null)
            {
                this.wrapperConfigurer.ConfigureFakeToWrap(result, options.WrappedInstance, options.SelfInitializedFakeRecorder);
            }

            return result;
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            logger.Debug("Trying to create dummy.");

            return this.session.TryResolveDummyValue(typeOfDummy, out result);
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            logger.Debug("Trying to create fake.");

            result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: false);

            if (options.WrappedInstance != null)
            {
                this.wrapperConfigurer.ConfigureFakeToWrap(result, options.WrappedInstance, options.SelfInitializedFakeRecorder);
            }

            return result != null;
        }
    }
}