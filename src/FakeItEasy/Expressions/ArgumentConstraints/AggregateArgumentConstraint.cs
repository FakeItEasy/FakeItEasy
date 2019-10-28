namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    internal class AggregateArgumentConstraint
        : IArgumentConstraint
    {
        private readonly IArgumentConstraint[] constraintsField;

        public AggregateArgumentConstraint(IEnumerable<IArgumentConstraint> constraints)
        {
            this.constraintsField = constraints.ToArray();
        }

        public IEnumerable<IArgumentConstraint> Constraints => this.constraintsField;

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write("[");

            bool first = true;

            foreach (var constraint in this.Constraints)
            {
                if (!first)
                {
                    writer.Write(", ");
                }
                else
                {
                    first = false;
                }

                constraint.WriteDescription(writer);
            }

            writer.Write("]");
        }

        public bool IsValid(object? argument)
        {
            var enumerable = argument as IEnumerable;

            if (enumerable is null || enumerable.Cast<object>().Count() != this.constraintsField.Length)
            {
                return false;
            }

            return enumerable.Cast<object>()
                .Zip(this.Constraints, (x, y) => new { Value = x, Constraint = y })
                .All(x => x.Constraint.IsValid(x.Value));
        }
    }
}
