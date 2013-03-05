namespace FakeItEasy
{
    using System;
    
    /// <summary>
    /// An attribute that can be applied to code that should be fixed because there's a
    /// code smell.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SmellyAttribute // NOTE (adamralph): LOL
        : Attribute
    {
        /// <summary>
        /// Gets or sets the description of the smell.
        /// </summary>
        public string Description { get; set; }
    }
}