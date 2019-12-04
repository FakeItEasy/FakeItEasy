namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using FakeItEasy.Configuration;

    internal class ArgumentConstraintTrap
        : IArgumentConstraintTrapper
    {
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
        private static readonly Action<IArgumentConstraint> ThrowWhenUnpreparedToTrapConstraint = constraint =>
            throw new InvalidOperationException(ExceptionMessages.ArgumentConstraintCanOnlyBeUsedInCallSpecification);

        private static ThreadLocal<Action<IArgumentConstraint>> saveTrappedConstraintAction =
            new ThreadLocal<Action<IArgumentConstraint>>(() => ThrowWhenUnpreparedToTrapConstraint);

        public static void ReportTrappedConstraint(IArgumentConstraint constraint) =>
            saveTrappedConstraintAction.Value.Invoke(constraint);

        public IArgumentConstraint TrapConstraintOrCreate(
            Action actionThatProducesConstraint,
            Func<IArgumentConstraint> defaultConstraintFactory)
        {
            var trappedConstraints = new List<IArgumentConstraint>();

            saveTrappedConstraintAction.Value = trappedConstraints.Add;
            try
            {
                actionThatProducesConstraint.Invoke();
            }
            finally
            {
                saveTrappedConstraintAction.Value = ThrowWhenUnpreparedToTrapConstraint;
            }

            return trappedConstraints.Count == 0 ? defaultConstraintFactory.Invoke() :
                trappedConstraints.Count == 1 ? trappedConstraints[0] :
                throw new FakeConfigurationException(ExceptionMessages.TooManyArgumentConstraints(trappedConstraints[1]));
        }
    }
}
