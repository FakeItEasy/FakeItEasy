namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "Is spelled correctly.")]
    public static class A
    {
        private static IFakeConfigurationManager ConfigurationManager
        {
            get { return ServiceLocator.Current.Resolve<IFakeConfigurationManager>(); }
        }

        private static IFakeCreatorFacade FakeCreator
        {
            get { return ServiceLocator.Current.Resolve<IFakeCreatorFacade>(); }
        }

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specifie the type of fake.")]
        public static T Fake<T>()
        {
            return FakeCreator.CreateFake<T>(x => { });
        }

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="options">A lambda where options for the built fake object cna be specified.</param>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specifie the type of fake.")]
        public static T Fake<T>(Action<IFakeOptionsBuilder<T>> options)
        {
            return FakeCreator.CreateFake(options);
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specifie the type of fake.")]
        public static IList<T> CollectionOfFake<T>(int numberOfFakes)
        {
            return FakeCreator.CollectionOfFake<T>(numberOfFakes);
        }

        /// <summary>
        /// Gets a dummy object of the specified type. The value of a dummy object
        /// should be irrelevant. Dummy objects should not be configured.
        /// </summary>
        /// <typeparam name="T">The type of dummy to return.</typeparam>
        /// <returns>A dummy object of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specifie the type of dummy.")]
        public static T Dummy<T>()
        {
            return FakeCreator.CreateDummy<T>();
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Uses the same naming as the framework")]
        public static new bool Equals(object objA, object objB)
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
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Uses the same naming as the framework")]
        public static new bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <param name="callSpecification">An expression where the configured memeber is called.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <typeparam name="T">The type of member on the faked object to configure.</typeparam>
        /// <param name="callSpecification">An expression where the configured memeber is called.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }
    }

    /// <summary>
    /// Provides an api entry point for constraining arguments of fake object calls.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "It is spelled correctly.")]
    public static class A<T>
    {
        /// <summary>
        /// Gets an argument constraint object that will be used to constrain a method call argument.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
        public static ArgumentConstraintScope<T> That
        {
            get { return new RootArgumentConstraintScope<T>(); }
        }

        /// <summary>
        /// Returns a constraint that considers any value of an argument as valid.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is a special case where the type parameter acts as an entry point into the fluent api.")]
        public static ArgumentConstraint<T> Ignored
        {
            get { return ArgumentConstraint.Create(new RootArgumentConstraintScope<T>(), x => true, "Ignored"); }
        }
    }
}