namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Analysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;

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
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
        public static T Fake<T>()
        {
            return FakeCreator.CreateFake<T>(x => { });
        }

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="optionsBuilder">A lambda where options for the built fake object can be specified.</param>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
        public static T Fake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            return FakeCreator.CreateFake(optionsBuilder);
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
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
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of dummy.")]
        public static T Dummy<T>()
        {
            return FakeCreator.CreateDummy<T>();
        }

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of dummies to create.</typeparam>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>A collection of dummy objects of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of dummy.")]
        public static IList<T> CollectionOfDummy<T>(int numberOfDummies)
        {
            return FakeCreator.CollectionOfDummy<T>(numberOfDummies);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <param name="callSpecification">An expression where the configured member is called.</param>
        /// <returns>A configuration object.</returns>
        [MustUseReturnValue(Diagnostics.UnusedCallSpecification)]
        public static IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Gets a configuration object allowing for further configuration of
        /// any call to the specified faked object.
        /// </summary>
        /// <param name="fake">
        /// The fake to configure.
        /// </param>
        /// <returns>
        /// A configuration object.
        /// </returns>
        [MustUseReturnValue(Diagnostics.UnusedCallSpecification)]
        public static IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fake)
        {
            return ConfigurationManager.CallTo(fake);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <typeparam name="T">The type of member on the faked object to configure.</typeparam>
        /// <param name="callSpecification">An expression where the configured member is called.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [MustUseReturnValue(Diagnostics.UnusedCallSpecification)]
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Configures a property setter on a faked object.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertySpecification">An expression that uses the getter aspect of the property to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [MustUseReturnValue(Diagnostics.UnusedCallSpecification)]
        public static IPropertySetterAnyValueConfiguration<TValue> CallToSet<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            return ConfigurationManager.CallToSet(propertySpecification);
        }
    }
}
