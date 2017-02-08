namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides extension methods for <see cref="IReturnValueConfiguration{T}"/> to configure async methods to return a failed task.
    /// </summary>
    public static class AsyncReturnValueConfigurationExtensions
    {
        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exception">The exception to set on the returned task when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
            this IReturnValueConfiguration<Task> configuration,
            Exception exception)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exception, nameof(exception));

            return configuration.ReturnsLazily(call => TaskHelper.FromException(exception));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
            this IReturnValueConfiguration<Task> configuration,
            Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily(call => TaskHelper.FromException(exceptionFactory(call)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
            this IReturnValueConfiguration<Task> configuration,
            Func<Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily(call => TaskHelper.FromException(exceptionFactory()));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T1">The type of the parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync<T1>(
            this IReturnValueConfiguration<Task> configuration,
            Func<T1, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily((T1 arg1) => TaskHelper.FromException(exceptionFactory(arg1)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync<T1, T2>(
            this IReturnValueConfiguration<Task> configuration,
            Func<T1, T2, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2) => TaskHelper.FromException(exceptionFactory(arg1, arg2)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync<T1, T2, T3>(
            this IReturnValueConfiguration<Task> configuration,
            Func<T1, T2, T3, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2, T3 arg3) => TaskHelper.FromException(exceptionFactory(arg1, arg2, arg3)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the exception factory.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync<T1, T2, T3, T4>(
                this IReturnValueConfiguration<Task> configuration,
                Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
                            TaskHelper.FromException(exceptionFactory(arg1, arg2, arg3, arg4)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exception">The exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Exception exception)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exception, nameof(exception));

            return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exception));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exceptionFactory(call)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Func<Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exceptionFactory()));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <typeparam name="T1">The type of the parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T, T1>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Func<T1, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return configuration.ReturnsLazily((T1 arg1) => TaskHelper.FromException<T>(exceptionFactory(arg1)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T, T1, T2>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Func<T1, T2, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2) => TaskHelper.FromException<T>(exceptionFactory(arg1, arg2)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T, T1, T2, T3>(
            this IReturnValueConfiguration<Task<T>> configuration,
            Func<T1, T2, T3, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2, T3 arg3) => TaskHelper.FromException<T>(exceptionFactory(arg1, arg2, arg3)));
        }

        /// <summary>
        /// Returns a failed task with the specified exception when the currently configured call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
        /// <typeparam name="T">The type of the returned task's result.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the exception factory.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the exception factory.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the exception factory.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the exception factory.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T, T1, T2, T3, T4>(
                this IReturnValueConfiguration<Task<T>> configuration,
                Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(exceptionFactory, nameof(exceptionFactory));

            return
                configuration.ReturnsLazily(
                    (T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
                            TaskHelper.FromException<T>(exceptionFactory(arg1, arg2, arg3, arg4)));
        }
    }
}
