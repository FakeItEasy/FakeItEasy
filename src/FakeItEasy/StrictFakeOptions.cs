namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Options for strict fakes.
    /// </summary>
    [Flags]
    public enum StrictFakeOptions
    {
        /// <summary>
        /// Default behavior. No unconfigured method can be called, including those
        /// inherited from <see cref="Object" />.
        /// </summary>
        None = 0,

        /// <summary>
        /// Calls to <see cref="Object.Equals(object)"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowEquals = 1,

        /// <summary>
        /// Calls to <see cref="Object.GetHashCode()"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowGetHashCode = 2,

        /// <summary>
        /// Calls to <see cref="Object.ToString()"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowToString = 4,

        /// <summary>
        /// Calls to all methods inherited from <see cref="Object"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowObjectMethods = AllowEquals | AllowGetHashCode | AllowToString
    }
}
