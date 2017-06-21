namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Used to tag fields and properties that will be initialized as a SUT through the Fake.Initialize-method.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Allows consumers to create their own testing DSL.")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Obsolete("Test fixture initialization will be removed in version 5.0.0.")]
    public class UnderTestAttribute : Attribute
    {
    }
}
