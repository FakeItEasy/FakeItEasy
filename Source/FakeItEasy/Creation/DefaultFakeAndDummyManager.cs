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

        public DefaultFakeAndDummyManager(IDummyValueCreationSession session, FakeObjectCreator fakeCreator)
        {
            this.session = session;
            this.fakeCreator = fakeCreator;
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
            return this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: true);
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            return this.session.TryResolveDummyValue(typeOfDummy, out result);
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: false);
            return result != null;
        }
    }
}