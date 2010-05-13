namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    /// <summary>
    /// Provides static methods for accessing fake objects.
    /// </summary>
    public static partial class Fake
    {
        /// <summary>
        /// Gets the fake object that manages the faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to get the manager object for.</param>
        /// <returns>The fake object manager.</returns>
        [DebuggerStepThrough]
        public static FakeManager GetFakeManager(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            var accessor = fakedObject as IFakedProxy;

            if (accessor == null)
            {
                var message = ExceptionMessages.ConfiguringNonFakeObjectExceptionMessage.FormatInvariant(fakedObject.GetType());
                throw new ArgumentException(message, "fakedObject");
            }

            return accessor.FakeManager;
        }



        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static IDisposable CreateScope()
        {
            return FakeScope.Create();
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <param name="container">The container to use within the specified scope.</param>
        /// <returns>The created scope.</returns>
        public static IDisposable CreateScope(IFakeObjectContainer container)
        {
            return FakeScope.Create(container);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are the same reference.
        /// </summary>
        /// <param name="objA">The obj A.</param>
        /// <param name="objB">The obj B.</param>
        /// <returns>True if the objects are the same reference.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        /// <summary>
        /// Gets all the calls made to the specified fake object.
        /// </summary>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns>A collection containing the calls to the object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        public static IEnumerable<ICompletedFakeObjectCall> GetCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            return Fake.GetFakeManager(fakedObject).RecordedCallsInScope;
        }
    }

    /// <summary>
    /// Represents a fake object that provides an api for configuring a faked object, exposed by the
    /// FakedObject-property.
    /// </summary>
    /// <typeparam name="T">The type of the faked object.</typeparam>
    public class Fake<T> : IStartConfiguration<T>
    {
        #region Construction
        /// <summary>
        /// Creates a new fake object.
        /// </summary>
        public Fake()
        {
            this.FakedObject = CreateFake(x => { });
        }

        /// <summary>
        /// Creates a new fake object using the specified options.
        /// </summary>
        /// <param name="options">Options used to create the fake object.</param>
        public Fake(Action<IFakeOptionsBuilder<T>> options)
        {
            Guard.AgainstNull(options, "options");

            this.FakedObject = CreateFake(options);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the faked object.
        /// </summary>
        public T FakedObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all calls made to the faked object.
        /// </summary>
        public IEnumerable<ICompletedFakeObjectCall> RecordedCalls
        {
            get
            {
                return Fake.GetCalls(this.FakedObject);
            }
        }

        private IStartConfiguration<T> StartConfiguration
        {
            get
            {
                var factory = ServiceLocator.Current.Resolve<IStartConfigurationFactory>();
                return factory.CreateConfiguration<T>(Fake.GetFakeManager(this.FakedObject));
            }
        }

        private static IFakeCreator FakeCreator
        {
            get
            {
                return ServiceLocator.Current.Resolve<IFakeCreator>();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<T>> callSpecification)
        {
            return this.StartConfiguration.CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <typeparam name="TMember">The type of value the member returns.</typeparam>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<T, TMember>> callSpecification)
        {
            return this.StartConfiguration.CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures any call to the fake object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        public IAnyCallConfiguration AnyCall()
        {
            return this.StartConfiguration.AnyCall();
        }

        private static T CreateFake(Action<IFakeOptionsBuilder<T>> options)
        {
            return FakeCreator.CreateFake<T>(options);
        }
        #endregion
    }
}