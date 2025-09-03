namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;

    /// <summary>
    /// Provides static methods for accessing fake objects.
    /// </summary>
    public static class Fake
    {
        private static readonly IFakeManagerAccessor FakeManagerAccessor = ServiceLocator.Resolve<IFakeManagerAccessor>();

        /// <summary>
        /// Gets the fake object that manages the faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to get the manager object for.</param>
        /// <returns>The fake object manager.</returns>
        /// <exception cref="ArgumentException">If <paramref name="fakedObject"/> is not actually a faked object.</exception>
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "The term fake object does not refer to the type System.Object.")]
        public static FakeManager GetFakeManager(object fakedObject)
        {
            Guard.AgainstNull(fakedObject);

            return FakeManagerAccessor.GetFakeManager(fakedObject);
        }

        /// <summary>
        /// Gets all the calls made to the specified fake object.
        /// </summary>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns>A collection containing the calls to the object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "The term fake object does not refer to the type System.Object.")]
        public static IEnumerable<ICompletedFakeObjectCall> GetCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject);

            return FakeManagerAccessor.GetFakeManager(fakedObject).GetRecordedCalls();
        }

        /// <summary>
        /// Clears all recorded calls of the faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to clear the recorded calls of.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "The term fake object does not refer to the type System.Object.")]
        public static void ClearRecordedCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject);

            var manager = FakeManagerAccessor.GetFakeManager(fakedObject);
            manager.ClearRecordedCalls();
        }

        /// <summary>
        /// Gets the fake manager associated with the proxy, if any.
        /// </summary>
        /// <param name="potentialFake">The potential proxy to get the manager from.</param>
        /// <param name="fakeManager">The fake manager, or <c>null</c> if <paramref name="potentialFake"/> is not actually a faked object.</param>
        /// <returns><c>true</c> if <paramref name="potentialFake"/> is a faked object, else <c>false</c>.</returns>
        [DebuggerStepThrough]
        public static bool TryGetFakeManager(object potentialFake, [NotNullWhen(true)] out FakeManager? fakeManager)
        {
            Guard.AgainstNull(potentialFake);

            fakeManager = FakeManagerAccessor.TryGetFakeManager(potentialFake);
            return fakeManager is not null;
        }

        /// <summary>
        /// Check if an object is a fake.
        /// </summary>
        /// <param name="potentialFake">The object to test.</param>
        /// <returns><c>true</c> if <paramref name="potentialFake"/> is a faked object, else <c>false</c>.</returns>
        [DebuggerStepThrough]
        public static bool IsFake(object potentialFake)
        {
            Guard.AgainstNull(potentialFake);

            return TryGetFakeManager(potentialFake, out _);
        }

        /// <summary>
        /// Reset the fake to its initial state.
        /// </summary>
        /// <param name="fake">The fake object to reset.</param>
        /// <remarks>
        /// This method resets all changes made to the fake after it was created. This includes:
        /// <list type="bullet">
        /// <item><description>Call configurations</description></item>
        /// <item><description>State of auto-faked properties</description></item>
        /// <item><description>Recorded calls</description></item>
        /// <item><description>Interception listeners</description></item>
        /// </list>
        /// However, changes made during fake creation (using <see cref="IFakeOptions.ConfigureFake"/>) will be preserved.
        /// </remarks>
        public static void Reset(object fake)
        {
            Guard.AgainstNull(fake);

            FakeManagerAccessor.GetFakeManager(fake).RestoreInitialState();
        }
    }
}
