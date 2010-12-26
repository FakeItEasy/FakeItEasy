namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides access to all calls made to fake objects within a scope.
    /// Scopes calls so that only calls made within the scope are visible.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Being a collection is not the main purpose of this interface.")]
    public interface IFakeScope
        : IDisposable, IEnumerable<ICompletedFakeObjectCall>
    {
    }
}