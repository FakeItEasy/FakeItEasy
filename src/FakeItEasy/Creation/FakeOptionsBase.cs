namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    internal abstract class FakeOptionsBase<T> : IFakeOptions<T>, IFakeOptions
    {
        IFakeOptions IFakeOptions.ConfigureFake(Action<object> action)
        {
            return (IFakeOptions)this.ConfigureFake(fake => action(fake));
        }

        IFakeOptions IFakeOptions.WithAdditionalAttributes(params Expression<Func<Attribute>>[] attributes)
        {
            return (IFakeOptions)this.WithAdditionalAttributes(attributes);
        }

        IFakeOptions IFakeOptions.Implements(Type interfaceType)
        {
            return (IFakeOptions)this.Implements(interfaceType);
        }

        IFakeOptions IFakeOptions.Implements<TInterface>()
        {
            return (IFakeOptions)this.Implements<TInterface>();
        }

        public IFakeOptions<T> Implements<TInterface>()
        {
            return this.Implements(typeof(TInterface));
        }

        IFakeOptionsForWrappers IFakeOptions.Wrapping(object wrappedInstance)
        {
            return (IFakeOptionsForWrappers)this.Wrapping((T)wrappedInstance);
        }

        public IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
        {
            return this.WithArgumentsForConstructor(GetConstructorArgumentsFromExpression(constructorCall));
        }

        IFakeOptions IFakeOptions.WithArgumentsForConstructor<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            return (IFakeOptions)this.WithArgumentsForConstructor(GetConstructorArgumentsFromExpression(constructorCall));
        }

        IFakeOptions IFakeOptions.WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return (IFakeOptions)this.WithArgumentsForConstructor(argumentsForConstructor);
        }

        public abstract IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor);

        public abstract IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance);

        public abstract IFakeOptions<T> WithAdditionalAttributes(params Expression<Func<Attribute>>[] attributes);

        public abstract IFakeOptions<T> Implements(Type interfaceType);

        public abstract IFakeOptions<T> ConfigureFake(Action<T> action);

        private static IEnumerable<object> GetConstructorArgumentsFromExpression<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            AssertThatExpressionRepresentConstructorCall(constructorCall);
            return ((NewExpression)constructorCall.Body).Arguments.Select(argument => argument.Evaluate());
        }

        private static void AssertThatExpressionRepresentConstructorCall<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            if (constructorCall.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
            }

            if (typeof(TConstructor) != typeof(T))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    ExceptionMessages.WrongConstructorExpressionTypeMessage,
                    typeof(T).FullNameCSharp(),
                    typeof(TConstructor).FullNameCSharp()));
            }
        }
    }
}
