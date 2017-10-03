namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    internal class ArgumentValueFormatter
    {
        private readonly ConcurrentDictionary<Type, IArgumentValueFormatter> cachedFormatters;
        private readonly IEnumerable<IArgumentValueFormatter> typeFormatters;

        public ArgumentValueFormatter(IEnumerable<IArgumentValueFormatter> typeFormatters, StringBuilderOutputWriter.Factory outputWriterFactory)
        {
            this.cachedFormatters = new ConcurrentDictionary<Type, IArgumentValueFormatter>();

            this.typeFormatters = typeFormatters.Concat(
                new IArgumentValueFormatter[]
                    {
                        new DefaultStringFormatter(),
                        new DefaultEnumerableValueFormatter(outputWriterFactory),
                        new DefaultFormatter()
                    });
        }

        public virtual string GetArgumentValueAsString(object argumentValue)
        {
            if (argumentValue == null)
            {
                return "NULL";
            }

            var argumentType = argumentValue.GetType();

            var formatter = this.cachedFormatters.GetOrAdd(argumentType, this.ResolveTypeFormatter);

            return formatter.GetArgumentValueAsString(argumentValue);
        }

        private static int GetDistanceFromKnownType(Type comparedType, Type knownType)
        {
            if (knownType == comparedType)
            {
                return 0;
            }

            if (comparedType.GetTypeInfo().IsInterface && knownType.GetInterfaces().Contains(comparedType))
            {
                return 1;
            }

            var distance = 2;
            var currentType = knownType.GetTypeInfo().BaseType;
            while (currentType != null)
            {
                if (currentType == comparedType)
                {
                    return distance;
                }

                distance++;
                currentType = currentType.GetTypeInfo().BaseType;
            }

            return int.MaxValue;
        }

        private IArgumentValueFormatter ResolveTypeFormatter(Type forType)
        {
            return
                (from formatter in this.typeFormatters
                 where formatter.ForType.IsAssignableFrom(forType)
                 select formatter)
                .Min(f => new RangedFormatter(f, GetDistanceFromKnownType(f.ForType, forType)))
                .Formatter;
        }

        /// <summary>
        /// Holds a formatter as well as the distance between a type to be formatted
        /// and the type for which the formatted is registered.
        /// </summary>
        private class RangedFormatter : IComparable<RangedFormatter>
        {
            private readonly int distanceFromKnownType;

            public RangedFormatter(IArgumentValueFormatter formatter, int distanceFromKnownType)
            {
                this.Formatter = formatter;
                this.distanceFromKnownType = distanceFromKnownType;
            }

            public IArgumentValueFormatter Formatter { get; }

            public int CompareTo(RangedFormatter other)
            {
                Guard.AgainstNull(other, nameof(other));

                if (other.distanceFromKnownType == this.distanceFromKnownType)
                {
                    return other.Formatter.Priority.CompareTo(this.Formatter.Priority);
                }

                return this.distanceFromKnownType.CompareTo(other.distanceFromKnownType);
            }
        }

        private class DefaultFormatter
            : ArgumentValueFormatter<object>
        {
            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(object argumentValue)
            {
                Guard.AgainstNull(argumentValue, nameof(argumentValue));

                return argumentValue.ToString();
            }
        }

        private class DefaultEnumerableValueFormatter
            : ArgumentValueFormatter<IEnumerable>
        {
            private readonly StringBuilderOutputWriter.Factory outputWriterFactory;

            public DefaultEnumerableValueFormatter(StringBuilderOutputWriter.Factory outputWriterFactory)
            {
                this.outputWriterFactory = outputWriterFactory;
            }

            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(IEnumerable argumentValue)
            {
                Guard.AgainstNull(argumentValue, nameof(argumentValue));

                var writer = this.outputWriterFactory(new StringBuilder());
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
                Guard.AgainstNull(argumentValue, nameof(argumentValue));

                if (argumentValue.Length == 0)
                {
                    return "string.Empty";
                }

                return string.Concat("\"", argumentValue, "\"");
            }
        }
    }
}
