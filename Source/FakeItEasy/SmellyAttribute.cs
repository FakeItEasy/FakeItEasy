namespace FakeItEasy
{
    using System;

    /// <summary>
    /// An attribute that can be applied to code that should be fixed becuase theres a
    /// code smell.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal class SmellyAttribute
        : Attribute
    {
        /// <summary>
        /// A description of the smell.
        /// </summary>
        public string Description { get; set; }
    }
}
