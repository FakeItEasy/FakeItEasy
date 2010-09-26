namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentValueFormatter
    {
        private IEnumerable<IArgumentValueFormatter> typeFormatters;
        private Dictionary<Type, IArgumentValueFormatter> cachedFormatters;

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
            private Type knownType;

            public ClosestToThisTypeComparer(Type knownType)
            {
                this.knownType = knownType;
            }

            public int Compare(IArgumentValueFormatter x, IArgumentValueFormatter y)
            {
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
                int distance = 0;
                var currentType = this.knownType;

                if (IsInterfaceImplementedByKnownType(comparedType))
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
            : IArgumentValueFormatter
        {
            public Type ForType
            {
                get { return typeof(object); }
            }

            public string GetArgumentValueAsString(object argumentValue)
            {
                return argumentValue.ToString();
            }


            public int Priority
            {
                get { return int.MinValue; }
            }
        }

        private class DefaultStringFormatter
            : IArgumentValueFormatter
        {
            public Type ForType
            {
                get { return typeof(string); }
            }

            public int Priority
            {
                get { return int.MinValue; }
            }

            public string GetArgumentValueAsString(object argumentValue)
            {
                var value = (string)argumentValue;

                if (value.Length == 0)
                {
                    return "string.Empty";
                }

                return string.Concat("\"", value, "\"");
            }
        }
    }
}
