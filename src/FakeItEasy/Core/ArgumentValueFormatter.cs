namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

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

            try
            {
                return formatter.GetArgumentValueAsString(argumentValue);
            }
            catch (Exception ex) when (formatter.GetType().GetTypeInfo().Assembly != typeof(ArgumentValueFormatter).GetTypeInfo().Assembly)
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Custom argument value formatter '{formatter}'"), ex);
            }
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
                this.typeFormatters
                    .Where(formatter => formatter.ForType.IsAssignableFrom(forType))
                    .OrderBy(formatter => GetDistanceFromKnownType(formatter.ForType, forType))
                    .ThenByDescending(formatter => formatter.Priority)
                    .First();
        }

        private class DefaultFormatter
            : ArgumentValueFormatter<object>
        {
            public override Priority Priority => Priority.Internal;

            protected override string GetStringValue(object argumentValue)
            {
                Guard.AgainstNull(argumentValue, nameof(argumentValue));

                var manager = Fake.TryGetFakeManager(argumentValue);

                return manager?.FakeObjectDisplayName ?? argumentValue.ToString();
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

                var writer = this.outputWriterFactory.Invoke();
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
