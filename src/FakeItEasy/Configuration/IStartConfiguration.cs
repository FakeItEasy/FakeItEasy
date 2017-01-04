namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides methods for configuring a fake object.
    /// </summary>
    /// <typeparam name="TFake">The type of fake object.</typeparam>
    internal interface IStartConfiguration<TFake> : IHideObjectMembers
    {
        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <typeparam name="TMember">The type of the return value of the member.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object when the specified property is set.
        /// </summary>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="propertySpecification">An expression that specifies the property to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IPropertySetterAnyValueConfiguration<TValue> CallsToSet<TValue>(Expression<Func<TFake, TValue>> propertySpecification);

        /// <summary>
        /// Configures the behavior of the fake object when a call is made to any method on the
        /// object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAnyCallConfigurationWithNoReturnTypeSpecified AnyCall();
    }
}
