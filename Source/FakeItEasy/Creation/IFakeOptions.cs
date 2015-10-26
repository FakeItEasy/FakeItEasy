namespace FakeItEasy.Creation
{
    using System;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides options for generating fake object.
    /// Has reduced functionality when compared to <see cref="IFakeOptions{T}"/>,
    /// which should be used when the type of the fake being created is known.
    /// </summary>
    public interface IFakeOptions
        : IHideObjectMembers
    {
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
    }
}