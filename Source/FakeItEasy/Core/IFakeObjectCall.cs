namespace FakeItEasy.Core
{
    using System.Reflection;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Represents a call to a fake object.
    /// </summary>
    public interface IFakeObjectCall
    {
        /// <summary>
        /// Gets the method that's called.
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the arguments used in the call.
        /// </summary>
        ArgumentCollection Arguments { get; }

        /// <summary>
        /// Gets the faked object the call is performed on.
        /// </summary>
        object FakedObject { get; }
    }
}