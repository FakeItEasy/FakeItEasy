namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    internal interface IConfigurationFactory
    {
        IVoidArgumentValidationConfiguration CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule);

        IReturnValueArgumentValidationConfiguration<TMember> CreateConfiguration<TMember>(FakeManager fakeObject, BuildableCallRule callRule);

        IAnyCallConfiguration CreateAnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule callRule);
    }
}
