namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Manages attaching of argument constraints.
    /// </summary>
    /// <typeparam name="T">The type of argument to constrain.</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public interface IArgumentConstraintManager<T>
    {
        /// <summary>
        /// Inverts the logic of the matches method.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Not", Justification = "Part of the fluent syntax.")]
        IArgumentConstraintManager<T> Not { get; }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="predicate">The predicate that should constrain the argument.</param>
        /// <param name="descriptionWriter">An action that will be write a description of the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter);
    }
}