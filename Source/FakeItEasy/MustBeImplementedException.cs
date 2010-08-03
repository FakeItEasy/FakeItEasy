#if DEBUG
namespace FakeItEasy
{
    using System;

    /// <summary>
    /// An exception that can be thrown before a member has been
    /// implemented, will cause the build to fail when not built in 
    /// debug mode.
    /// </summary>
    public class MustBeImplementedException
        : Exception
    {
    }
}
#endif