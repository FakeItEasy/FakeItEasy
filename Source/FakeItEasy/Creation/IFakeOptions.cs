namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection.Emit;
    using Configuration;

    /// <summary>
    /// Provides options for generating fake object.
    /// Has reduced functionality when compared to <see cref="IFakeOptions{T}"/>,
    /// which should be used when the type of the fake being created is known.
    /// </summary>
    public interface IFakeOptions
        : IHideObjectMembers
    {
        /// <summary>
        /// Specifies that the fake should be created with these additional attributes.
        /// </summary>
        /// <param name="customAttributeBuilders">The attributes to build into the proxy.</param>
        /// <returns>Options object.</returns>
        IFakeOptions WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders);

        /// <summary>
        /// Specifies an action that should be run over the fake object for the initial configuration (during the creation of the fake proxy).
        /// </summary>
        /// <param name="action">An action to perform on the Fake.</param>
        /// <returns>Options object.</returns>
        /// <remarks>
        /// <para>
        /// Note that this method might be called when the fake is not yet fully constructed, so <paramref name="action"/> should
        /// use the fake instance to set up behavior, but not rely on the instance's state.
        /// Also, if FakeItEasy has to try multiple constructors in order
        /// to create the fake (for example, because one or more constructors throw exceptions and must be bypassed),
        /// the <c>action</c> will be called more than once, so it should be side effect-free.
        /// </para>
        /// </remarks>
        IFakeOptions ConfigureFake(Action<object> action);

        /// <summary>
        /// Sets up the fake to implement the specified interface in addition to the
        /// originally faked class.
        /// </summary>
        /// <param name="interfaceType">The type of interface to implement.</param>
        /// <returns>Options object.</returns>
        /// <exception cref="ArgumentException">The specified type is not an interface.</exception>
        /// <exception cref="ArgumentNullException">The specified type is null.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Implements", Justification = "Would be a breaking change, might be changed in a later major version.")]
        IFakeOptions Implements(Type interfaceType);
    }
}