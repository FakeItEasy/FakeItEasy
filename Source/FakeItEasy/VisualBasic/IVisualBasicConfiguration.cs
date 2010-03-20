namespace FakeItEasy.VisualBasic
{
    using System;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Configurations for visual basic.
    /// </summary>
    /// <typeparam name="TFake">The type of the configured faked object.</typeparam>
    public interface IVisualBasicConfiguration
        : IVoidConfiguration, IAssertConfiguration
    {
        
    }
}