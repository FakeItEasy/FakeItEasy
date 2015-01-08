namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides static methods for accessing fake objects.
    /// </summary>
    public static class Fake
    {
        private static FakeFacade Facade
        {
            get { return ServiceLocator.Current.Resolve<FakeFacade>(); }
        }

        /// <summary>
        /// Gets the fake object that manages the faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to get the manager object for.</param>
        /// <returns>The fake object manager.</returns>
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "The term fake object does not refer to the type System.Object.")]
        public static FakeManager GetFakeManager(object fakedObject)
        {
            return Facade.GetFakeManager(fakedObject);
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static IFakeScope CreateScope()
        {
            return Facade.CreateScope();
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <param name="container">The container to use within the specified scope.</param>
        /// <returns>The created scope.</returns>
        public static IFakeScope CreateScope(IFakeObjectContainer container)
        {
            return Facade.CreateScope(container);
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
            return Facade.GetCalls(fakedObject);
        }

        /// <summary>
        /// Clears the configuration of the faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to clear the configuration of.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "The term fake object does not refer to the type System.Object.")]
        public static void ClearConfiguration(object fakedObject)
        {
            Facade.ClearConfiguration(fakedObject);
        }

        /// <summary>
        /// Sets a new fake to each property or field that is tagged with the FakeAttribute in the specified
        /// fixture.
        /// </summary>
        /// <param name="fixture">The object to initialize.</param>
        public static void InitializeFixture(object fixture)
        {
            Facade.InitializeFixture(fixture);
        }
    }
}