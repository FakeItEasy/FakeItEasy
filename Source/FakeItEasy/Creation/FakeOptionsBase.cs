namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection.Emit;

    internal abstract class FakeOptionsBase<T> : IFakeOptions<T>, IFakeOptions
    {
        IFakeOptions IFakeOptions.ConfigureFake(Action<object> action)
        {
            return (IFakeOptions)this.ConfigureFake(fake => action(fake));
        }

        IFakeOptions IFakeOptions.WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            return (IFakeOptions)this.WithAdditionalAttributes(customAttributeBuilders);
        }

        public abstract IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor);

        public abstract IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall);

        public abstract IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance);

        public abstract IFakeOptions<T> WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders);

        public abstract IFakeOptions<T> Implements(Type interfaceType);

        public abstract IFakeOptions<T> Implements<TInterface>();

        public abstract IFakeOptions<T> ConfigureFake(Action<T> action);
    }
}