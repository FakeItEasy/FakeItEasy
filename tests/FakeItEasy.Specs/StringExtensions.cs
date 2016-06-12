namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Creation;
    using Xbehave;
    using Xbehave.Sdk;

    public static class StringExtensions
    {
        /// <summary>
        /// Creates a step builder that can be used to refer to a type.
        /// Useful when we want to establish a condition with actually performing an action.
        /// </summary>
        /// <typeparam name="T">The type to look at.</typeparam>
        /// <param name="text">A description of the type's relevant quality.</param>
        /// <returns>A step builder.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to indicate the type to see.")]
        public static IStepBuilder See<T>(this string text)
        {
            return text.x(() => { });
        }

        /// <summary>
        /// Creates a step builder that can be used to refer to a method that
        /// creates a type, such as a constructor.
        /// Useful when we want to establish a condition with actually performing an action.
        /// </summary>
        /// <typeparam name="T">The type to look at.</typeparam>
        /// <param name="text">A description of the method's relevant quality.</param>
        /// <param name="func">
        /// The constructor (or other method that creates an instance) to look at.
        /// Will not be executed.
        /// </param>
        /// <returns>A step builder.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "func", Justification = "Used to indicate the method to see.")]
        public static IStepBuilder See<T>(this string text, Func<T> func)
        {
            return text.x(() => { });
        }

        /// <summary>
        /// Creates a step builder that can be used to refer to a type's method.
        /// Useful when we want to establish a condition with actually performing an action.
        /// </summary>
        /// <typeparam name="T">The type to look at.</typeparam>
        /// <param name="text">A description of the method's relevant quality.</param>
        /// <param name="func">The method to look at. Will not be executed.</param>
        /// <returns>A step builder.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "func", Justification = "Used to indicate the method to see.")]
        public static IStepBuilder See<T>(this string text, Func<T, Action<Type, IFakeOptions>> func)
        {
            return text.x(() => { });
        }

        /// <summary>
        /// Creates a step builder that can be used to refer to something.
        /// Useful when we want to establish a condition with actually performing an action.
        /// </summary>
        /// <remarks>Should only be used when no type-safe variant of <c>See</c> applies.</remarks>
        /// <param name="text">A description of the thing's relevant quality.</param>
        /// <param name="thingToSee">The thing to look at.</param>
        /// <returns>A step builder.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "thingToSee", Justification = "Used to indicate the thing to see.")]
        public static IStepBuilder See(this string text, string thingToSee)
        {
            return text.x(() => { });
        }
    }
}
