namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides extension methods for fake objects.
    /// </summary>
    public static class FakeExtensions
    {
        private const string NameOfInvokesFeature = "invokes";

        private const string NameOfReturnsLazilyFeature = "returns lazily";

        private const string NameOfThrowsFeature = "throws";

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
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object.</returns>
        public static TInterface WithAnyArguments<TInterface>(this IArgumentValidationConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

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

            configuration.MustHaveHappened(Repeated.AtLeast.Once);
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
        /// <typeparam name="T">
        /// The type of return value.
        /// </typeparam>
        /// <param name="configuration">
        /// The call configuration to extend.
        /// </param>
        /// <param name="values">
        /// The values to return in sequence.
        /// </param>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<T> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, "configuration");

            var queue = new Queue<T>(values);
            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }

        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
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
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily<T>(this IReturnValueConfiguration<T> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(valueProducer, "valueProducer");

            return configuration.ReturnsLazily(x => valueProducer());
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer" /> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3, T4>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, T4, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Writes the calls in the collection to the specified text writer.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        /// <param name="writer">The writer to write the calls to.</param>
        public static void Write<T>(this IEnumerable<T> calls, IOutputWriter writer) where T : IFakeObjectCall
        {
            Guard.AgainstNull(calls, "calls");
            Guard.AgainstNull(writer, "writer");

            var callWriter = ServiceLocator.Current.Resolve<CallWriter>();
            callWriter.WriteCalls(calls.Cast<IFakeObjectCall>(), writer);
        }

        /// <summary>
        /// Writes all calls in the collection to the console.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        public static void WriteToConsole<T>(this IEnumerable<T> calls) where T : IFakeObjectCall
        {
            calls.Write(new DefaultOutputWriter(Console.Write));
        }

        /// <summary>
        /// Gets the argument at the specified index in the arguments collection
        /// for the call.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="call">The call to get the argument from.</param>
        /// <param name="argumentIndex">The index of the argument.</param>
        /// <returns>The value of the argument with the specified index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generic argument is used to cast the result.")]
        public static T GetArgument<T>(this IFakeObjectCall call, int argumentIndex)
        {
            Guard.AgainstNull(call, "call");

            return call.Arguments.Get<T>(argumentIndex);
        }

        /// <summary>
        /// Gets the argument with the specified name in the arguments collection
        /// for the call.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="call">The call to get the argument from.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <returns>The value of the argument with the specified name.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generic argument is used to cast the result.")]
        public static T GetArgument<T>(this IFakeObjectCall call, string argumentName)
        {
            Guard.AgainstNull(call, "call");
            Guard.AgainstNull(argumentName, "argumentName");

            return call.Arguments.Get<T>(argumentName);
        }

        /// <summary>
        /// Makes the fake strict, this means that any call to the fake
        /// that has not been explicitly configured will throw an exception.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">The configuration.</param>
        /// <returns>A configuration object.</returns>
        public static IFakeOptionsBuilder<T> Strict<T>(this IFakeOptionsBuilder<T> options)
        {
            Guard.AgainstNull(options, "options");

            Action<IFakeObjectCall> thrower = c =>
                {
                    throw new ExpectationException("Call to non configured method \"{0}\" of strict fake.".FormatInvariant(c.Method.Name));
                };

            return options.OnFakeCreated(
                x => A.CallTo(x).Invokes(thrower));
        }

        /// <summary>
        /// Applies a predicate to constrain which calls will be considered for interception.
        /// </summary>
        /// <typeparam name="T">
        /// The return type of the where method.
        /// </typeparam>
        /// <param name="configuration">
        /// The configuration object to extend.
        /// </param>
        /// <param name="predicate">
        /// A predicate for a fake object call.
        /// </param>
        /// to the output.
        /// <returns>
        /// The configuration object.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Appropriate for expressions.")]
        public static T Where<T>(this IWhereConfiguration<T> configuration, Expression<Func<IFakeObjectCall, bool>> predicate)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(predicate, "predicate");

            return configuration.Where(predicate.Compile(), x => x.Write(predicate.ToString()));
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made. This overload can also be used to fake calls with arguments when they don't need to be accessed.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action" /> to invoke.</param>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake>(this ICallbackConfiguration<TFake> configuration, Action actionToInvoke)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Invokes(call => actionToInvoke());
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1>(this ICallbackConfiguration<TFake> configuration, Action<T1> actionToInvoke)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Invokes(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, actionToInvoke.Method, NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2> actionToInvoke)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Invokes(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, actionToInvoke.Method, NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2, T3>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2, T3> actionToInvoke)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Invokes(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, actionToInvoke.Method, NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3,T4}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2, T3, T4>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2, T3, T4> actionToInvoke)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Invokes(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, actionToInvoke.Method, NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exception">The exception to throw when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration Throws(this IExceptionThrowerConfiguration configuration, Exception exception)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => exception);
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <returns>Configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration Throws(this IExceptionThrowerConfiguration configuration, Func<Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => exceptionFactory());
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1>(this IExceptionThrowerConfiguration configuration, Func<T1, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2, T3>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, T3, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2, T3, T4>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    AssertThatSignaturesAreEqual(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <typeparam name="T">The type of exception to throw.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static IAfterCallSpecifiedConfiguration Throws<T>(this IExceptionThrowerConfiguration configuration) where T : Exception, new()
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => new T());
        }

        private static void AssertThatSignaturesAreEqual(MethodInfo callMethod, MethodInfo actionMethod, string nameOfFeature)
        {
            if (!SignaturesAreEqual(callMethod, actionMethod))
            {
                var fakeSignature = BuildSignatureDescription(callMethod);
                var actionSignature = BuildSignatureDescription(actionMethod);

                throw new FakeConfigurationException("The faked method has the signature ({0}), but {2} was used with ({1}).".FormatInvariant(fakeSignature, actionSignature, nameOfFeature));
            }
        }

        private static bool SignaturesAreEqual(MethodInfo methodOne, MethodInfo methodTwo)
        {
            return methodOne.GetParameters().Select(x => x.ParameterType).SequenceEqual(methodTwo.GetParameters().Select(x => x.ParameterType));
        }

        private static string BuildSignatureDescription(MethodInfo method)
        {
            return method.GetParameters().ToCollectionString(x => x.ParameterType.FullName, ", ");
        }
    }
}