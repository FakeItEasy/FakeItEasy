namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    internal class ArgumentValueFormatter
    {
        private readonly IEnumerable<IArgumentValueFormatter> typeFormatters;

        public ArgumentValueFormatter(IEnumerable<IArgumentValueFormatter> typeFormatters)
        {
            this.typeFormatters = typeFormatters.Concat(
                new IArgumentValueFormatter[]
                    {
                        new DefaultStringFormatter(),
                        new DefaultEnumerableValueFormatter(this),
                        new DefaultFormatter()
                    });
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any type of exception may be encountered.")]
        public virtual string GetArgumentValueAsString(object? argumentValue)
        {
            if (argumentValue is null)
            {
                return "NULL";
            }

            var argumentType = argumentValue.GetType();

            foreach (var formatter in this.GetTypeFormatterCandidates(argumentType))
            {
                try
                {
                    var formattedValue = formatter.GetArgumentValueAsString(argumentValue);
                    if (formattedValue is not null)
                    {
                        return formattedValue;
                    }
                }
                catch when (formatter.GetType().Assembly != typeof(ArgumentValueFormatter).Assembly)
                {
                    // We don't expect internal formatters to throw. If one does, letting the exception bubble up may
                    // inconvenience the users in the short term, but it's better that we learn about it.
                }
            }

            // There's a built-in formatter that matches any object, so we're guaranteed not to reach this point.
            // Return just to satisfy the compiler.
            return argumentType.ToString();
        }

        private static int GetDistanceFromKnownType(Type comparedType, Type knownType)
        {
            if (knownType == comparedType)
            {
                return 0;
            }

            if (comparedType.IsInterface && knownType.GetInterfaces().Contains(comparedType))
            {
                return 1;
            }

            var distance = 2;
            var currentType = knownType.BaseType;
            while (currentType is not null)
            {
                if (currentType == comparedType)
                {
                    return distance;
                }

                distance++;
                currentType = currentType.BaseType;
            }

            return int.MaxValue;
        }

        private IEnumerable<IArgumentValueFormatter> GetTypeFormatterCandidates(Type forType) =>
            this.typeFormatters
                .Where(formatter => formatter.ForType.IsAssignableFrom(forType))
                .OrderBy(formatter => GetDistanceFromKnownType(formatter.ForType, forType))
                .ThenByDescending(formatter => formatter.Priority);

        private class DefaultFormatter
            : ArgumentValueFormatter<object>
        {
            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(object argumentValue)
            {
                Guard.AgainstNull(argumentValue);

                return Fake.TryGetFakeManager(argumentValue, out var manager)
                    ? manager.FakeObjectDisplayName
                    : argumentValue.ToString() ?? string.Empty;
            }
        }

        private class DefaultEnumerableValueFormatter
            : ArgumentValueFormatter<IEnumerable>
        {
            private readonly ArgumentValueFormatter formatter;

            public DefaultEnumerableValueFormatter(ArgumentValueFormatter formatter)
            {
                this.formatter = formatter;
            }

            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(IEnumerable argumentValue)
            {
                Guard.AgainstNull(argumentValue);

                var writer = new StringBuilderOutputWriter(this.formatter);
                writer.Write("[");
                writer.WriteArgumentValues(argumentValue);
                writer.Write("]");
                return writer.Builder.ToString();
            }
        }

        private class DefaultStringFormatter
            : ArgumentValueFormatter<string>
        {
            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(string argumentValue)
            {
                Guard.AgainstNull(argumentValue);

                if (argumentValue.Length == 0)
                {
                    return "string.Empty";
                }

                return string.Concat("\"", argumentValue, "\"");
            }
        }
    }
}
