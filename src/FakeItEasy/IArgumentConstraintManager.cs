namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Manages attaching of argument constraints.
    /// </summary>
    /// <typeparam name="T">The type of argument to constrain.</typeparam>
    public interface IArgumentConstraintManager<T> : IHideObjectMembers
    {
        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="predicate">The predicate that should constrain the argument.</param>
        /// <param name="descriptionWriter">An action that will be write a description of the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter);
    }
}
