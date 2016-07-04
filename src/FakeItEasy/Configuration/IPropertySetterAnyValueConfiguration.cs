namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides configuration for property setters and allows the user to specify validations for arguments.
    /// </summary>
    /// <typeparam name="TValue">The value of the property.</typeparam>
    public interface IPropertySetterAnyValueConfiguration<TValue> :
        IPropertySetterConfiguration, IArgumentValidationConfiguration<IPropertySetterConfiguration>
    {
        /// <summary>
        /// Configures the property assignment to be accepted when the value <see cref="object.Equals(object)"/>
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to match.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(To), Justification = "There's no need for clients to implement the member.")]
        IPropertySetterConfiguration To(TValue value);

        /// <summary>
        /// Configures the property assignment to be accepted when the value satisfies <paramref name="valueConstraint"/>.
        /// This overload would be most useful when using <see cref="A{T}.Ignored"/> or <see cref="A{T}.That"/>.
        /// </summary>
        /// <param name="valueConstraint">The value constraint to satisfy.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(To), Justification = "There's no need for clients to implement the member.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IPropertySetterConfiguration To(Expression<Func<TValue>> valueConstraint);
    }
}
