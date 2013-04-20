namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentValueFormatter
    {
        private readonly Dictionary<Type, IArgumentValueFormatter> cachedFormatters;
        private readonly IEnumerable<IArgumentValueFormatter> typeFormatters;

        public ArgumentValueFormatter(IEnumerable<IArgumentValueFormatter> typeFormatters)
        {
            this.cachedFormatters = new Dictionary<Type, IArgumentValueFormatter>();

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
            if (!this.cachedFormatters.TryGetValue(argumentType, out formatter))
            {
                formatter = this.ResolveTypeFormatter(argumentType);
                this.cachedFormatters.Add(argumentType, formatter);
            }

            return formatter.GetArgumentValueAsString(argumentValue);
        }

        private IArgumentValueFormatter ResolveTypeFormatter(Type forType)
        {
            return this.GetFormattersThatSupportsTypeOrderedBySpecificity(forType).First();
        }

        private IEnumerable<IArgumentValueFormatter> GetFormattersThatSupportsTypeOrderedBySpecificity(Type type)
        {
            return
                (from formatter in this.typeFormatters
                 where formatter.ForType.IsAssignableFrom(type)
                 select formatter)
                    .OrderBy(x => x, new ClosestToThisTypeComparer(type));
        }

        private class ClosestToThisTypeComparer
            : IComparer<IArgumentValueFormatter>
        {
            private readonly Type knownType;

            public ClosestToThisTypeComparer(Type knownType)
            {
                this.knownType = knownType;
            }

            public int Compare(IArgumentValueFormatter x, IArgumentValueFormatter y)
            {
                Guard.AgainstNull(x, "x");
                Guard.AgainstNull(y, "y");

                var distanceOfX = this.GetDistanceFromKnownType(x.ForType);
                var distanceOfY = this.GetDistanceFromKnownType(y.ForType);

                if (distanceOfX == distanceOfY)
                {
                    return y.Priority.CompareTo(x.Priority);
                }

                return distanceOfX.CompareTo(distanceOfY);
            }

            private int GetDistanceFromKnownType(Type comparedType)
            {
                var distance = 0;
                var currentType = this.knownType;

                if (this.IsInterfaceImplementedByKnownType(comparedType))
                {
                    return 0;
                }

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

            private bool IsInterfaceImplementedByKnownType(Type comparedType)
            {
                return comparedType.IsInterface &&
                    this.knownType.GetInterfaces().Contains(comparedType);
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