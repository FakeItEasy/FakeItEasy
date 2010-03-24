namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides options for generating fake object.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    public interface IFakeOptionsBuilder<T>
        : IHideObjectMembers
    {
        //IFakeBuilderOptionsBuilder<T> Implementing<TInterface>();

        /// <summary>
        /// Specifies arguments for the constructor of the faked class.
        /// </summary>
        /// <param name="argumentsForConstructor">The arguments to pass to the consturctor of the faked class.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilder<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Specifies arguments for the constructor of the faked class by giving an expression with the call to
        /// the desired constructor using the arguments to be passed to the constructor.
        /// </summary>
        /// <param name="constructorCall">The constructor call to use when creating a class proxy.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall);

        /// <summary>
        /// Specifies that the fake should delegate calls to the specified instance.
        /// </summary>
        /// <param name="wrappedInstance">The object to delegate calls to.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance);

        /// <summary>
        /// Sets up the fake to implement the specified interface in addition to the
        /// originally faked class.
        /// </summary>
        /// <param name="interfaceType">The type of interface to implement.</param>
        /// <returns>Options object.</returns>
        /// <exception cref="ArgumentException">The specified type is not an interface.</exception>
        /// <exception cref="ArgumentNullException">The specified type is null.</exception>
        IFakeOptionsBuilder<T> Implements(Type interfaceType);
    }
}
