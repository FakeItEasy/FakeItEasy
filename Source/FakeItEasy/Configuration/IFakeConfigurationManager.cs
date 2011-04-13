namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Handles the configuration of fake object given an expression specifying
    /// a call on a faked object.
    /// </summary>
    internal interface IFakeConfigurationManager
    {
        IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification);

        IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification);

        IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fakeObject);
    }
}