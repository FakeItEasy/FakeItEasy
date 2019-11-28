namespace FakeItEasy.Tests
{
    using System;
    using System.Collections;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    public static class CustomArgumentConstraints
    {
        public static T IsThisSequence<T>(this IArgumentConstraintManager<T> scope, T collection) where T : IEnumerable
        {
            return scope.Matches(
                x => x.Cast<object>().SequenceEqual(collection.Cast<object>()),
                "This sequence: " + collection.Cast<object>().ToCollectionString(x => x.ToString(), ", "));
        }

        public static T IsThisSequence<T>(this IArgumentConstraintManager<T> scope, params object[] collection)
            where T : IEnumerable
        {
            return scope.Matches(
                x => x is object && x.Cast<object>().SequenceEqual(collection),
                "This sequence: " + collection.ToCollectionString(x => x.ToString(), ", "));
        }

        public static FakeManager Fakes(this IArgumentConstraintManager<FakeManager> scope, object fake)
        {
            return scope.Matches(x => x.Equals(Fake.GetFakeManager(fake)), "Specified FakeObject");
        }

        internal static ParsedArgumentExpression ProducesValue(
            this IArgumentConstraintManager<ParsedArgumentExpression> scope, object expectedValue)
        {
            return scope.Matches(
                x => object.Equals(expectedValue, x.Expression.Evaluate()),
                "Expression that produces the value " + expectedValue);
        }

        internal static IProxyOptions IsEmpty(this IArgumentConstraintManager<IProxyOptions> scope)
        {
            return scope.NullCheckedMatches(
                x => !x.AdditionalInterfacesToImplement.Any()
                     && x.ArgumentsForConstructor is null
                     && !x.ProxyConfigurationActions.Any()
                     && !x.Attributes.Any(),
                x => x.Write("empty fake options"));
        }

        internal static Action<IOutputWriter> Writes(
            this IArgumentConstraintManager<Action<IOutputWriter>> manager,
            string expectedValue)
        {
            return manager.NullCheckedMatches(
                x =>
                {
                    var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
                    x.Invoke(writer);

                    return string.Equals(writer.Builder.ToString(), expectedValue, StringComparison.Ordinal);
                },
                x => x.Write("action that writes ").WriteArgumentValue(expectedValue).Write(" to output."));
        }

        internal static Func<TInput, TExpected> Returns<TInput, TExpected>(
            this IArgumentConstraintManager<Func<TInput, TExpected>> manager,
            TInput inputValue,
            TExpected expectedValue)
        {
            return manager.NullCheckedMatches(
                x => object.Equals(x.Invoke(inputValue), expectedValue),
                x =>
                {
                    x.Write("a function that returns ").WriteArgumentValue(expectedValue);
                });
        }
    }
}
