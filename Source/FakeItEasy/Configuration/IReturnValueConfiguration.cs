namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// Configures a call that returns a value.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    /// <typeparam name="TMember">The type of the member.</typeparam>
    public interface IReturnValueConfiguration<TMember>
        : IExceptionThrowerConfiguration, 
          ICallbackConfiguration<IReturnValueConfiguration<TMember>>, 
          IAssertConfiguration,
          IHideObjectMembers,
          ICallBaseConfiguration
    {
        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(TMember value);

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(Func<TMember> valueProducer);

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns(Func<IFakeObjectCall, TMember> valueProducer);
    }
}