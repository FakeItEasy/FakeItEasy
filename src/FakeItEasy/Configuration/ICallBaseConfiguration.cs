namespace FakeItEasy.Configuration
{
    using System;

    /// <summary>
    /// Configuration that lets you specify that a fake object call should call it's base method.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface ICallBaseConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// When the configured method or methods are called the call
        /// will be delegated to the base method of the faked method.
        /// </summary>
        /// <returns>A configuration object.</returns>
        /// <exception cref="InvalidOperationException">The fake object is of an abstract type or an interface
        /// and no base method exists.</exception>
        IAfterCallConfiguredConfiguration<TInterface> CallsBaseMethod();
    }
}
