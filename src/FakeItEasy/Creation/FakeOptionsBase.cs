namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal abstract class FakeOptionsBase<T> : IFakeOptions<T>, IFakeOptions
    {
        IFakeOptions IFakeOptions.ConfigureFake(Action<object> action)
        {
            return (IFakeOptions)this.ConfigureFake(fake => action(fake));
        }

        IFakeOptions IFakeOptions.WithAttributes(params Expression<Func<Attribute>>[] attributes)
        {
            return (IFakeOptions)this.WithAttributes(attributes);
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

#if FEATURE_SELF_INITIALIZED_FAKES
        IFakeOptionsForWrappers IFakeOptions.Wrapping(object wrappedInstance)
        {
            return (IFakeOptionsForWrappers)this.Wrapping((T)wrappedInstance);
        }
#else
        IFakeOptions IFakeOptions.Wrapping(object wrappedInstance)
        {
            return (IFakeOptions)this.Wrapping((T)wrappedInstance);
        }
#endif

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

#if FEATURE_SELF_INITIALIZED_FAKES
        public abstract IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance);
#else
        public abstract IFakeOptions<T> Wrapping(T wrappedInstance);
#endif

        public abstract IFakeOptions<T> WithAttributes(params Expression<Func<Attribute>>[] attributes);

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
                throw new ArgumentException(ExceptionMessages.NonConstructorExpression);
            }

            if (typeof(TConstructor) != typeof(T))
            {
                throw new ArgumentException(
                    ExceptionMessages.WrongConstructorExpressionType(
                        typeof(T),
                        typeof(TConstructor)));
            }
        }
    }
}
