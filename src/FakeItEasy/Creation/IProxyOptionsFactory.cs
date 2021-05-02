namespace FakeItEasy.Creation
{
    using System;

    internal interface IProxyOptionsFactory
    {
        IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions>? optionsBuilder);
    }
}
