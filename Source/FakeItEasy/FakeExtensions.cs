namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
using System.IO;

    /// <summary>
    /// Provides extension methods for fake objects.
    /// </summary>
    public static class FakeExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");
            configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.NumberOfTimes(2);
        }

        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object</returns>
        public static TInterface WithAnyArguments<TInterface>(this IArgumentValidationConfiguration<TInterface> configuration)
        {
            return configuration.WhenArgumentsMatch(x => true);
        }

        /// <summary>
        /// Filters to contain only the calls that matches the call specification.
        /// </summary>
        /// <typeparam name="TFake">The type of fake the call is made on.</typeparam>
        /// <param name="calls">The calls to filter.</param>
        /// <param name="callSpecification">The call to match on.</param>
        /// <returns>A collection of the calls that matches the call specification.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The compiler would not be able to figure out the type.")]
        public static IEnumerable<ICompletedFakeObjectCall> Matching<TFake>(this IEnumerable<ICompletedFakeObjectCall> calls, Expression<Action<TFake>> callSpecification)
        {
            var factory = ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>();
            var matcher = factory.CreateCallMathcer(callSpecification);

            return
                from call in calls
                where matcher.Matches(call)
                select call;
        }

        /// <summary>
        /// Asserts that the specified call must have happened once or more.
        /// </summary>
        /// <param name="configuration">The configuration to assert on.</param>
        public static void MustHaveHappened(this IAssertConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.MustHaveHappened(Repeated.Once);
        }

        /// <summary>
        /// Asserts that the specified has not happened.
        /// </summary>
        /// <param name="configuration">The configuration to assert on.</param>
        public static void MustNotHaveHappened(this IAssertConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.MustHaveHappened(Repeated.Never);
        }

        /// <summary>
        /// Configures the call to return the next value from the specified sequence each time it's called. Null will
        /// be returned when all the values in the sequence has been returned.
        /// </summary>
        /// <typeparam name="T">The type of return value.</typeparam>
        /// <param name="configuration">The call configuration to extend.</param>
        /// <param name="values">The values to return in sequence.</param>
        /// <returns>A configuration object.</returns>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<T> configuration, params T[] values)
        {
            var queue = new Queue<T>(values);

            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }

        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns<T>(this IReturnValueConfiguration<T> configuration, T value)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(x => value);
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily<T>(this IReturnValueConfiguration<T> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(valueProducer, "valueProducer");

            return configuration.ReturnsLazily(x => valueProducer());
        }

        /// <summary>
        /// Writes the calls in the collection to the specified text writer.
        /// </summary>
        /// <param name="calls">The calls to write.</param>
        /// <param name="writer">The writer to write the calls to.</param>
        public static void Write(this IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var callWriter = ServiceLocator.Current.Resolve<CallWriter>();
            callWriter.WriteCalls(0, calls, writer);
        }

        /// <summary>
        /// Writes all calls in the collection to the console.
        /// </summary>
        /// <param name="calls">The calls to write.</param>
        public static void WriteToConsole(this IEnumerable<IFakeObjectCall> calls)
        {
            Guard.AgainstNull(calls, "calls");

            calls.Write(Console.Out);
        }
    }
}