namespace FakeItEasy.Core
{
    using System;

    internal interface IArgumentConstraintTrapper
    {
        IArgumentConstraint TrapConstraint(Action actionThatProducesConstraint);
    }
}
