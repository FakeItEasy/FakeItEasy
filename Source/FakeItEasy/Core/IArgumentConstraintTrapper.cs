namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal interface IArgumentConstraintTrapper
    {
        IEnumerable<IArgumentConstraint> TrapConstraints(Action actionThatProducesConstraint);
    }
}