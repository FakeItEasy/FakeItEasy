namespace FakeItEasy.ExtensionSyntax.Full
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for configuring and asserting on faked objects
    /// without going through the static methods of the Fake-class.
    /// </summary>
    public static class FullExtensionSyntax
    {
        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <typeparam name="TMember">The type of the return value of the member.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <typeparam name="TFake">The type of fake object to configure.</typeparam>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TFake, TMember>(this TFake fakedObject, Expression<Func<TFake, TMember>> callSpecification)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");
            Guard.AgainstNull(callSpecification, "callSpecification");
            
            return fakedObject.Configure().CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <typeparam name="TFake">The type of fake object to configure.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IVoidArgumentValidationConfiguration CallsTo<TFake>(this TFake fakedObject, Expression<Action<TFake>> callSpecification)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");
            Guard.AgainstNull(callSpecification, "callSpecification");

            return fakedObject.Configure().CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures the behavior of the fake object when a call is made to any method on the
        /// object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidConfiguration AnyCall<TFake>(this TFake fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            return fakedObject.Configure().AnyCall();
        }
    }
}
