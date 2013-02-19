namespace FakeItEasy.Core
{
    using System;

    internal interface ISutInitializer
    {
        object CreateSut(Type typeOfSut, Action<Type, object> onFakeCreated);
    }
}