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
        /// inherited from <see cref="object" />.
        /// </summary>
        None = 0,

        /// <summary>
        /// Calls to <see cref="object.Equals(object)"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowEquals = 1,

        /// <summary>
        /// Calls to <see cref="object.GetHashCode()"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowGetHashCode = 2,

        /// <summary>
        /// Calls to <see cref="object.ToString()"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowToString = 4,

        /// <summary>
        /// Calls to all methods inherited from <see cref="object"/> are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowObjectMethods = AllowEquals | AllowGetHashCode | AllowToString,

        /// <summary>
        /// Calls to event accessors are allowed, and behave as if the fake weren't strict.
        /// </summary>
        AllowEvents = 8
    }
}
