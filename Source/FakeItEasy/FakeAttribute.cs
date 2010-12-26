namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Used to tag fields and properties that will be initialized through the
    /// Fake.Initialize-method.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Users should be able to subclass this attribute to be able to adjust naming.")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FakeAttribute : Attribute
    {
    }
}