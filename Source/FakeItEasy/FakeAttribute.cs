namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Used to tag fields and properties that will be initialized through the
    /// Fake.Initialize-method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FakeAttribute : Attribute
    {
    }
}
