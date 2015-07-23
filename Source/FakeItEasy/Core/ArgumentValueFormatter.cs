namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentValueFormatter
    {
        private readonly ConcurrentDictionary<Type, IArgumentValueFormatter> cachedFormatters;
        private readonly IEnumerable<IArgumentValueFormatter> typeFormatters;

        public ArgumentValueFormatter(IEnumerable<IArgumentValueFormatter> typeFormatters)
        {
            this.cachedFormatters = new ConcurrentDictionary<Type, IArgumentValueFormatter>();

            this.typeFormatters = typeFormatters.Concat(
                new IArgumentValueFormatter[]
                    {
                        new DefaultStringFormatter(), 
                        new DefaultFormatter()
                    });
        }

        public virtual string GetArgumentValueAsString(object argumentValue)
        {
            if (argumentValue == null)
            {
                return "<NULL>";
            }

            var argumentType = argumentValue.GetType();

            IArgumentValueFormatter formatter;
            formatter = this.cachedFormatters.GetOrAdd(argumentType, this.ResolveTypeFormatter);

            return formatter.GetArgumentValueAsString(argumentValue);
        }

        private static int GetDistanceFromKnownType(Type comparedType, Type knownType)
        {
            if (knownType.Equals(comparedType))
            {
                return 0;
            }

            if (comparedType.IsInterface && knownType.GetInterfaces().Contains(comparedType))
            {
                return 1;
            }

            var distance = 2;
            var currentType = knownType.BaseType;
            while (currentType != null)
            {
                if (currentType.Equals(comparedType))
                {
                    return distance;
                }

                distance++;
                currentType = currentType.BaseType;
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

            public IArgumentValueFormatter Formatter { get; private set; }

            public int CompareTo(RangedFormatter other)
            {
                Guard.AgainstNull(other, "other");

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
            protected override string GetStringValue(object argumentValue)
            {
                Guard.AgainstNull(argumentValue, "argumentValue");

                return argumentValue.ToString();
            }
        }

        private class DefaultStringFormatter
            : ArgumentValueFormatter<string>
        {
            protected override string GetStringValue(string argumentValue)
            {
                Guard.AgainstNull(argumentValue, "argumentValue");

                if (argumentValue.Length == 0)
                {
                    return "string.Empty";
                }

                return string.Concat("\"", argumentValue, "\"");
            }
        }
    }
}