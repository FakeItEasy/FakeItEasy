namespace FakeItEasy.Creation
{
    using System;

    internal interface IDummyValueResolver
    {
        bool TryResolveDummyValue(Type typeOfDummy, out object result);
    }
}
