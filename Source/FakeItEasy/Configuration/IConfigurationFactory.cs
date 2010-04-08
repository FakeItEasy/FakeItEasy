namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    internal interface IConfigurationFactory
    {
        IVoidArgumentValidationConfiguration CreateConfiguration(FakeObject fakeObject, BuildableCallRule callRule);

        IReturnValueArgumentValidationConfiguration<TMember> CreateConfiguration<TMember>(FakeObject fakeObject, BuildableCallRule callRule);

        IAnyCallConfiguration CreateAnyCallConfiguration(FakeObject fakeObject, AnyCallCallRule callRule);
    }
}
