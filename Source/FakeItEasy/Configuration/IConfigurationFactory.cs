namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    internal interface IConfigurationFactory
    {
        IVoidArgumentValidationConfiguration CreateConfiguration(FakeManager fakeObject, BuildableCallRule callRule);

        IAnyCallConfigurationWithReturnTypeSpecified<TMember> CreateConfiguration<TMember>(FakeManager fakeObject, BuildableCallRule callRule);

        IAnyCallConfigurationWithNoReturnTypeSpecified CreateAnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule callRule);
    }
}
