namespace FakeItEasy.Core;

using System;

internal interface IArgumentConstraintTrapper
{
    IArgumentConstraint TrapConstraintOrCreate(
        Action actionThatProducesConstraint,
        Func<IArgumentConstraint> constraintFactory);
}
