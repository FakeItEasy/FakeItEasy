namespace FakeItEasy.Creation
{
    using System;

    internal interface IDummyValueResolver
    {
        CreationResult TryResolveDummyValue(Type typeOfDummy);
    }
}
