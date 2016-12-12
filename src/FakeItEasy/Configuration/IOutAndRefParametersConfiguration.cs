namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// Lets the developer configure output values of out and ref parameters.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IOutAndRefParametersConfiguration<out TInterface> : IHideObjectMembers
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
        IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer);

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily<T1>(Func<T1, object[]> valueProducer);

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily<T1, T2>(Func<T1, T2, object[]> valueProducer);

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily<T1, T2, T3>(Func<T1, T2, T3, object[]> valueProducer);

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object[]> valueProducer);
    }
}
