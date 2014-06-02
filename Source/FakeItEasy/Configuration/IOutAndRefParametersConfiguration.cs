namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    /// <summary>
    /// Lets the developer configure output values of out and ref parameters.
    /// </summary>
    public interface IOutAndRefParametersConfiguration
    {
        /// <summary>
        /// Specifies a function used to produce the output values for the out and ref parameters.
        /// Specify the values in the order the ref and out parameters have in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer);
    }
}