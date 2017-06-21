namespace FakeItEasy
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Enables negation of argument constraints.
    /// </summary>
    /// <typeparam name="T">The type of argument to constrain.</typeparam>
    public interface INegatableArgumentConstraintManager<T> : IArgumentConstraintManager<T>
    {
        /// <summary>
        /// Inverts the logic of the subsequent constraint.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Not", Justification = "Part of the fluent syntax.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
        IArgumentConstraintManager<T> Not { get; }
    }
}
