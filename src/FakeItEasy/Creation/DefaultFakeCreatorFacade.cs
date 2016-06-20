namespace FakeItEasy.Creation
{
    using System;

    internal class DefaultFakeCreatorFacade
        : IFakeCreatorFacade
    {
        private readonly IFakeAndDummyManager fakeAndDummyManager;

        public DefaultFakeCreatorFacade(IFakeAndDummyManager fakeAndDummyManager)
        {
            this.fakeAndDummyManager = fakeAndDummyManager;
        }

        public T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder, "optionsBuilder");

            return (T)this.fakeAndDummyManager.CreateFake(typeof(T), options => optionsBuilder((IFakeOptions<T>)options));
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            Guard.AgainstNull(typeOfFake, nameof(typeOfFake));
            Guard.AgainstNull(optionsBuilder, nameof(optionsBuilder));

            return this.fakeAndDummyManager.CreateFake(typeOfFake, optionsBuilder);
        }

        public T CreateDummy<T>()
        {
            return (T)this.fakeAndDummyManager.CreateDummy(typeof(T));
        }

        public object CreateDummy(Type typeOfDummy)
        {
            Guard.AgainstNull(typeOfDummy, nameof(typeOfDummy));

            return this.fakeAndDummyManager.CreateDummy(typeOfDummy);
        }
    }
}
