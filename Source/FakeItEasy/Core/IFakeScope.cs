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
        /// <summary>
        /// Adds a call rule to all fake objects used in scope. The rule is used for all calles in scope including fakes created outside of scope.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddScopeRuleFirst(IFakeObjectCallRule rule);

        /// <summary>
        /// Adds a call rule last in the list of scope rules, meaning it has the lowest priority possible.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddScopeRuleLast(IFakeObjectCallRule rule);
    }
}