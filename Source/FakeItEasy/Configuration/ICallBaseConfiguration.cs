namespace FakeItEasy.Configuration
{
    using System;

    /// <summary>
    /// Configuration that lets you specify that a fake object call should call it's base method.
    /// </summary>
    public interface ICallBaseConfiguration
        : IHideObjectMembers
    {
        /// <summary>
        /// When the configured method or methods are called the call
        /// will be delegated to the base method of the faked method.
        /// </summary>
        /// <returns>A configuration object.</returns>
        /// <exception cref="InvalidOperationException">The fake object is of an abstract type or an interface
        /// and no base method exists.</exception>
        IAfterCallSpecifiedConfiguration CallsBaseMethod();
    }
}