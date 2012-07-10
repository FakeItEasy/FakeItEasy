namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    internal class AggregateArgumentConstraint
        : IArgumentConstraint
    {
        public AggregateArgumentConstraint(IEnumerable<IArgumentConstraint> constraints)
        {
            this.Constraints = constraints;
        }

        public IEnumerable<IArgumentConstraint> Constraints { get; set; }

        public void WriteDescription(IOutputWriter writer)
        {
        }

        public bool IsValid(object argument)
        {
            var pairedArgumentsAndConstraints = ((IEnumerable)argument).Cast<object>().Zip(this.Constraints, (argumentValue, constraint) => new { argumentValue, constraint });
            return pairedArgumentsAndConstraints.All(x => x.constraint.IsValid(x.argumentValue));
        }
    }

}