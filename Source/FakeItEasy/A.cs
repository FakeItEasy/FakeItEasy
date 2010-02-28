namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;
    using FakeItEasy.SelfInitializedFakes;
    using FakeItEasy.Configuration;
    using System.Reflection;

    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    public static class A
    {
        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        public static T Fake<T>()
        {
            return CreateFake<T>(null);
        }

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="options">A lambda where options for the built fake object cna be specified.</param>
        /// <returns>A fake object.</returns>
        public static T Fake<T>(Action<IFakeBuilderOptionsBuilder<T>> options)
        {
            var generator = ServiceLocator.Current.Resolve<IFakeObjectBuilder>();
            return generator.GenerateFake<T>(options);
        }

        /// <summary>
        /// Gets a dummy object of the specified type. The value of a dummy object
        /// should be irrelevant. Dummy objects should not be configured.
        /// </summary>
        /// <typeparam name="T">The type of dummy to return.</typeparam>
        /// <returns>A dummy object of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static T Dummy<T>()
        {
            return (T)Factory.CreateFake(typeof(T), null, true);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        public static new bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        private static FakeObjectFactory Factory
        {
            get
            {
                return ServiceLocator.Current.Resolve<FakeObjectFactory>();
            }
        }

        private static T CreateFake<T>(IEnumerable<object> argumentsForConstructor)
        {
            return (T)CreateFake(typeof(T), argumentsForConstructor);
        }

        private static object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            return Factory.CreateFake(typeOfFake, argumentsForConstructor, false);
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
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        private static IFakeConfigurationManager ConfigurationManager
        {
            get
            {
                return ServiceLocator.Current.Resolve<IFakeConfigurationManager>();
            }
        }
    }

    /// <summary>
    /// Provides an api entry point for validating arguments of fake object calls.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    public class A<T>
    {
        /// <summary>
        /// Gets an argument validations object that provides validations for the argument.
        /// </summary>
        public static ArgumentConstraintScope<T> That
        {
            get
            {
                return new RootValidations<T>();
            }
        }

        /// <summary>
        /// Returns a validator that considers any value of an argument as valid.
        /// </summary>
        public static ArgumentConstraint<T> Ignored
        {
            get
            {
                return ArgumentConstraint.Create(new RootValidations<T>(), x => true, "Ignored");
            }
        }
    }
}