namespace FakeItEasy.Core
{
    using System;

    internal class DefaultArgumentConstraintManager<T>
        : INegatableArgumentConstraintManager<T>
    {
        private readonly Action<IArgumentConstraint> onConstraintCreated;

        public DefaultArgumentConstraintManager(Action<IArgumentConstraint> onConstraintCreated)
        {
            this.onConstraintCreated = onConstraintCreated;
        }

        public IArgumentConstraintManager<T> Not => new NotArgumentConstraintManager(this);

        public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.onConstraintCreated(new MatchesConstraint(predicate, descriptionWriter));
            return default!;
        }

        private class NotArgumentConstraintManager
            : IArgumentConstraintManager<T>
        {
            private readonly IArgumentConstraintManager<T> parent;

            public NotArgumentConstraintManager(IArgumentConstraintManager<T> parent)
            {
                this.parent = parent;
            }

            public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                return this.parent.Matches(
                    x => !predicate(x),
                    x =>
                    {
                        x.Write("not ");
                        descriptionWriter.Invoke(x);
                    });
            }
        }

        private class MatchesConstraint
            : ITypedArgumentConstraint
        {
            private static readonly bool IsNullable = typeof(T).IsNullable();

            private readonly Func<T, bool> predicate;
            private readonly Action<IOutputWriter> descriptionWriter;

            public MatchesConstraint(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.predicate = predicate;
                this.descriptionWriter = descriptionWriter;
            }

            public Type Type => typeof(T);

            public override string ToString() => this.GetDescription();

            void IArgumentConstraint.WriteDescription(IOutputWriter writer)
            {
                writer.Write("<");
                try
                {
                    this.descriptionWriter.Invoke(writer);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Argument matcher description"), ex);
                }

                writer.Write(">");
            }

            bool IArgumentConstraint.IsValid(object? argument)
            {
                if (!IsValueValidForType(argument))
                {
                    return false;
                }

                try
                {
                    return this.predicate.Invoke((T)argument!);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Argument matcher {this.GetDescription()}"), ex);
                }
            }

            private static bool IsValueValidForType(object? argument)
            {
                if (argument is null)
                {
                    return IsNullable;
                }

                return argument is T;
            }

            private string GetDescription()
            {
                var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
                ((IArgumentConstraint)this).WriteDescription(writer);
                return writer.Builder.ToString();
            }
        }
    }
}
