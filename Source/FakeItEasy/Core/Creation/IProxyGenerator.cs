namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a generator that can generate proxies that emits events when calls are intercepted.
    /// </summary>
    public interface IProxyGenerator
    {
        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container);

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Gets a value indicating if the specified member can be intercepted on a proxied
        /// instance.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if the member can be intercepted, otherwise false.</returns>
        bool MemberCanBeIntercepted(MemberInfo member);
    }

    /// <summary>
    /// Represents a generator that can generate proxies that emits events when calls are intercepted.
    /// </summary>
    public interface IProxyGeneratorNew
    {
        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <param name="additionalInterfacesToImplement">Any extra interfaces to be implemented by the generated proxy.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        ProxyResult GenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Gets a value indicating if the specified member can be intercepted on a proxied
        /// instance.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if the member can be intercepted, otherwise false.</returns>
        bool MemberCanBeIntercepted(MemberInfo member);
    }
}
