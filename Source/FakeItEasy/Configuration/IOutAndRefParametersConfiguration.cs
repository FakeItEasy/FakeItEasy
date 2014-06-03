namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// Lets the developer configure output values of out and ref parameters.
    /// </summary>
    public interface IOutAndRefParametersConfiguration
    {
        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer);
    }
}