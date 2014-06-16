namespace FakeItEasy.Core
{
    using System;

    internal class DefaultArgumentConstraintManager<T>
        : IArgumentConstraintManager<T>
    {
        private readonly Action<IArgumentConstraint> onConstraintCreated;

        public DefaultArgumentConstraintManager(Action<IArgumentConstraint> onConstraintCreated)
        {
            this.onConstraintCreated = onConstraintCreated;
        }

        public IArgumentConstraintManager<T> Not
        {
            get { return new NotArgumentConstraintManager(this); }
        }

        public T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.onConstraintCreated(new MatchesConstraint(predicate, descriptionWriter));
            return default(T);
        }

        private class NotArgumentConstraintManager
            : IArgumentConstraintManager<T>
        {
            private readonly IArgumentConstraintManager<T> parent;

            public NotArgumentConstraintManager(IArgumentConstraintManager<T> parent)
            {
                this.parent = parent;
            }

            public IArgumentConstraintManager<T> Not
            {
                get { return new NotArgumentConstraintManager(this); }
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
            : IArgumentConstraint
        {
            private readonly Func<T, bool> predicate;
            private readonly Action<IOutputWriter> descriptionWriter;

            public MatchesConstraint(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.predicate = predicate;
                this.descriptionWriter = descriptionWriter;
            }

            void IArgumentConstraint.WriteDescription(IOutputWriter writer)
            {
                writer.Write("<");
                this.descriptionWriter.Invoke(writer);
                writer.Write(">");
            }

            bool IArgumentConstraint.IsValid(object argument)
            {
                return (argument == null || argument is T) && this.predicate.Invoke((T)argument);
            }
        }
    }
}