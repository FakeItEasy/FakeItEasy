namespace FakeItEasy.Core
{
    using System;

    internal class DefaultArgumentConstraintManager<T>
        : ICapturableArgumentConstraintManager<T>
    {
        private readonly Action<MatchesConstraint> onConstraintCreated;

        public DefaultArgumentConstraintManager(Action<IArgumentConstraint> onConstraintCreated)
            : this((MatchesConstraint constraint) => onConstraintCreated(constraint))
        {
        }

        private DefaultArgumentConstraintManager(Action<MatchesConstraint> onConstraintCreated) =>
            this.onConstraintCreated = onConstraintCreated;

        public IArgumentConstraintManager<T> Not =>
            new DefaultArgumentConstraintManager<T>(
                constraint => this.onConstraintCreated(new NotMatchesConstraint(constraint)));

        public INegatableArgumentConstraintManager<T> IsCapturedTo<TCapture>(Captured<T, TCapture> capturedArgument) =>
            new DefaultArgumentConstraintManager<T>(
                constraint => this.onConstraintCreated(new CapturesConstraint<TCapture>(constraint, capturedArgument)));

        public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.onConstraintCreated(new MatchesConstraint(predicate, descriptionWriter));
            return default!;
        }

        private class NotMatchesConstraint(MatchesConstraint constraint) : MatchesConstraint(
                  argument => !constraint.IsValid(argument),
                  writer =>
                      {
                          writer.Write("not ");
                          constraint.WriteBareDescription(writer);
                      })
        {
        }

        private class CapturesConstraint<TCapture>(MatchesConstraint constraint, Captured<T, TCapture> capturedArgument)
            : MatchesConstraint(argument => constraint.IsValid(argument), constraint.WriteBareDescription),
              IHaveASideEffect
        {
            public void ApplySideEffect(object? argument) => capturedArgument.Add((T)argument!);
        }

        private class MatchesConstraint(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
                        : ITypedArgumentConstraint
        {
            private static readonly bool IsNullable = typeof(T).IsNullable();

            public Type Type => typeof(T);

            public override string ToString() => this.GetDescription();

            public void WriteDescription(IOutputWriter writer)
            {
                writer.Write('<');
                this.WriteBareDescription(writer);
                writer.Write('>');
            }

            public bool IsValid(object? argument)
            {
                if (!IsValueValidForType(argument))
                {
                    return false;
                }

                try
                {
                    return predicate.Invoke((T)argument!);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Argument matcher {this.GetDescription()}"), ex);
                }
            }

            public void WriteBareDescription(IOutputWriter writer)
            {
                try
                {
                    descriptionWriter.Invoke(writer);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Argument matcher description"), ex);
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
