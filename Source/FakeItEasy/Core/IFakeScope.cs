namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Provides access to all calls made to fake objects within a scope.
    /// Scopes calls so that only calls made within the scope are visible.
    /// </summary>
    public interface IFakeScope
        : IDisposable, IEnumerable<ICompletedFakeObjectCall>
    {

    }
}
