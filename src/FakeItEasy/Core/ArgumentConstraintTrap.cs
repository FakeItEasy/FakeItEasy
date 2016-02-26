namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal class ArgumentConstraintTrap
        : IArgumentConstraintTrapper
    {
        [ThreadStatic]
        private static List<IArgumentConstraint> trappedConstraints;

        public static void ReportTrappedConstraint(IArgumentConstraint constraint)
        {
            if (trappedConstraints == null)
            {
                throw new InvalidOperationException("A<T>.Ignored, A<T>._, and A<T>.That can only be used in the context of a call specification with A.CallTo()");
            }

            trappedConstraints.Add(constraint);
        }

        public IEnumerable<IArgumentConstraint> TrapConstraints(Action actionThatProducesConstraint)
        {
            trappedConstraints = new List<IArgumentConstraint>();
            var result = trappedConstraints;

            actionThatProducesConstraint.Invoke();

            trappedConstraints = null;

            return result;
        }
    }
}
