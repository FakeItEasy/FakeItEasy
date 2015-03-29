namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides options for generating fake object.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    public interface IFakeOptionsBuilder<T>
        : IHideObjectMembers
    {
        /// <summary>
        /// Specifies arguments for the constructor of the faked class.
        /// </summary>
        /// <param name="argumentsForConstructor">The arguments to pass to the constructor of the faked class.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilder<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Specifies arguments for the constructor of the faked class by giving an expression with the call to
        /// the desired constructor using the arguments to be passed to the constructor.
        /// </summary>
        /// <param name="constructorCall">The constructor call to use when creating a class proxy.</param>
        /// <returns>Options object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        IFakeOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall);

        /// <summary>
        /// Specifies that the fake should delegate calls to the specified instance.
        /// </summary>
        /// <param name="wrappedInstance">The object to delegate calls to.</param>
        /// <returns>Options object.</returns>
        /// <remarks>
        /// <para>
        /// If both <c>Wrapping</c> and <see cref="ConfigureFake"/> are used when creating a fake,
        /// the <c>ConfigureFake</c> actions will take precedence for the methods they apply to.
        /// </para>
        /// <para>
        /// When a fake is created for a type that has an <see cref="IFakeConfigurator"/>, and <c>Wrapping</c> is 
        /// used to wrap another object, the <c>Wrapping</c> directive takes precedence.
        /// </para>
        /// </remarks>
        IFakeOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance);

        /// <summary>
        /// Specifies that the fake should be created with these additional attributes.
        /// </summary>
        /// <param name="customAttributeBuilders">The attributes to build into the proxy.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilder<T> WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders);

        /// <summary>
        /// Sets up the fake to implement the specified interface in addition to the
        /// originally faked class.
        /// </summary>
        /// <param name="interfaceType">The type of interface to implement.</param>
        /// <returns>Options object.</returns>
        /// <exception cref="ArgumentException">The specified type is not an interface.</exception>
        /// <exception cref="ArgumentNullException">The specified type is null.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Implements", Justification = "Would be a breaking change, might be changed in a later major version.")]
        IFakeOptionsBuilder<T> Implements(Type interfaceType);

        /// <summary>
        /// Sets up the fake to implement the specified interface in addition to the
        /// originally faked class.
        /// </summary>
        /// <typeparam name="TInterface">The type of interface to implement.</typeparam>
        /// <returns>Options object.</returns>
        /// <exception cref="ArgumentException">The specified type is not an interface.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Implements", Justification = "Would be a breaking change, might be changed in a later major version.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to provide a strongly typed fluent API.")]
        IFakeOptionsBuilder<T> Implements<TInterface>();

        /// <summary>
        /// Specifies an action that should be run over the fake object for the initial configuration (during the creation of the fake proxy).
        /// </summary>
        /// <param name="action">An action to perform.</param>
        /// <returns>Options object.</returns>
        /// <remarks>
        /// <para>
        /// Note that this method might be called when the fake is not yet fully constructed, so <paramref name="action"/> should
        /// use the fake instance to set up behavior, but not rely on the instance's state.
        /// Also, if FakeItEasy has to try multiple constructors in order
        /// to create the fake (for example, because one or more constructors throw exceptions and must be bypassed),
        /// the <c>action</c> will be called more than once, so it should be side effect-free.
        /// </para>
        /// <para>
        /// If both <see cref="Wrapping"/> and <c>ConfigureFake</c> are used when creating a fake,
        /// the <c>ConfigureFake</c> actions will take precedence for the methods they apply to.
        /// </para>
        /// <para>
        /// When a fake is created for a type that has an <see cref="IFakeConfigurator"/>, and <c>ConfigureFake</c> is 
        /// used to configure the fake as well, the <c>ConfigureFake</c> actions take precedence.
        /// </para>
        /// </remarks>
        IFakeOptionsBuilder<T> ConfigureFake(Action<T> action);
    }
}