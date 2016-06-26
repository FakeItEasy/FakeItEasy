namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides extension methods for <see cref="ICompletedFakeObjectCall"/>.
    /// </summary>
    public static class CompletedFakeObjectCallExtensions
    {
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
            var callExpressionParser = ServiceLocator.Current.Resolve<ICallExpressionParser>();
            var matcher = factory.CreateCallMatcher(callExpressionParser.Parse(callSpecification));

            return
                from call in calls
                where matcher.Matches(call)
                select call;
        }
    }
}
