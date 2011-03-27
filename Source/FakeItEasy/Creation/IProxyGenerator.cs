namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// An interface to be implemented by classes that can generate proxies for FakeItEasy.
    /// </summary>
    public interface IProxyGenerator
    {
        /// <summary>
        /// Generates a proxy of the specifed type and returns a result object containing information
        /// about the success of the generation and the proxy if it was generated.
        /// </summary>
        /// <param name="typeOfProxy">The type of proxy to generate.</param>
        /// <param name="additionalInterfacesToImplement">Interfaces to be implemented by the proxy.</param>
        /// <param name="argumentsForConstructor">Arguments to pass to the constructor of the type in <paramref name="typeOfProxy" />.</param>
        /// <returns>A result containging the generated proxy.</returns>
        ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Gets a value indicating if the specified member can be intercepted by the proxy generator.
        /// </summary>
        /// <param name="method">The member to test.</param>
        /// <param name="callTarget">The instance the method will be called on.</param>
        /// <param name="failReason">The reason the method can not be intercepted.</param>
        /// <returns>True if the member can be intercepted.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Seems appropriate here.")]
        bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason);
    }
}