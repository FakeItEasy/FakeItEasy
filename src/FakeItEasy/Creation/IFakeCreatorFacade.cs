namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeCreatorFacade
    {
        T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder);

        object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder);

        T CreateDummy<T>();

        object CreateDummy(Type typeOfDummy);
    }
}
