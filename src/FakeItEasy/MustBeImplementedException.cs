#if DEBUG

namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An exception that can be thrown before a member has been
    /// implemented, will cause the build to fail when not built in
    /// debug mode.
    /// </summary>
#if FEATURE_SERIALIZATION
    [Serializable]
#endif
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Never used in production.")]
    public class MustBeImplementedException
        : Exception
    {
    }
}

#endif
