namespace FakeItEasy
{
    using System;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Extension methods to allow more convenient Invokes definition.
    /// </summary>
    public static class InvokesExtensions
    {
        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action"/> to invoke</param>
        /// <exception cref="FakeConfigurationException"> when the signatures of the faked method and the <paramref name="actionToInvoke"/> do not match</exception>
        public static TInterface Invokes<TInterface>(this ICallbackConfiguration<TInterface> configuration, Action actionToInvoke)
        {

            return configuration.Invokes(
                call =>
                    {
                        EnsureSignature(call.Method, actionToInvoke.Method);

                        actionToInvoke();
                    });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1}"/> to invoke</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call</typeparam>
        /// <exception cref="FakeConfigurationException"> when the signatures of the faked method and the <paramref name="actionToInvoke"/> do not match</exception>
        public static TInterface Invokes<TInterface, T1>(this ICallbackConfiguration<TInterface> configuration, Action<T1> actionToInvoke)
        {
            return configuration.Invokes(
                call =>
                    {
                        EnsureSignature(call.Method, actionToInvoke.Method);

                        actionToInvoke(call.GetArgument<T1>(0));
                    });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2}"/> to invoke</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call</typeparam>
        /// <exception cref="FakeConfigurationException"> when the signatures of the faked method and the <paramref name="actionToInvoke"/> do not match</exception>
        public static TInterface Invokes<TInterface, T1, T2>(this ICallbackConfiguration<TInterface> configuration, Action<T1, T2> actionToInvoke)
        {
            return configuration.Invokes(
                call =>
                    {
                        EnsureSignature(call.Method, actionToInvoke.Method);

                        actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                    });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3}"/> to invoke</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call</typeparam>
        /// <exception cref="FakeConfigurationException"> when the signatures of the faked method and the <paramref name="actionToInvoke"/> do not match</exception>
        public static TInterface Invokes<TInterface, T1, T2, T3>(this ICallbackConfiguration<TInterface> configuration, Action<T1, T2, T3> actionToInvoke)
        {
            return configuration.Invokes(
                call =>
                    {
                        EnsureSignature(call.Method, actionToInvoke.Method);

                        actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                    });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3,T4}"/> to invoke</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call</typeparam>
        /// <exception cref="FakeConfigurationException"> when the signatures of the faked method and the <paramref name="actionToInvoke"/> do not match</exception>
        public static TInterface Invokes<TInterface, T1, T2, T3, T4>(this ICallbackConfiguration<TInterface> configuration, Action<T1, T2, T3, T4> actionToInvoke)
        {
            return configuration.Invokes(
                call =>
                    {
                        EnsureSignature(call.Method, actionToInvoke.Method);

                        actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                    });
        }

        private static void EnsureSignature(MethodInfo callMethod, MethodInfo actionMethod)
        {
            var fakeSignature = BuildSignature(callMethod);
            var actionSignature = BuildSignature(actionMethod);

            if (fakeSignature != actionSignature)
            {
                throw new FakeConfigurationException("The faked method has the signature " + fakeSignature + ", \r\nbut invokes was used with          " + actionSignature + ".");
            }
        }

        private static string BuildSignature(MethodInfo method)
        {
            var s = new StringBuilder();
            s.Append("(");

            var isFirst = true;
            foreach (var parameter in method.GetParameters())
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    s.Append(", ");
                }

                s.Append(parameter.ParameterType.FullName);
            }

            return s.Append(")").ToString();
        }
    }
}