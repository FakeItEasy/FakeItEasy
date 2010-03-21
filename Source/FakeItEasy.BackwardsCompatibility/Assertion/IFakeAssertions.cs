namespace FakeItEasy.Assertion
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides methods for asserting on fake object calls.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    public interface IFakeAssertions<TFake>
        : IHideObjectMembers
    {
        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <param name="callSpecification"></param>
        void WasCalled(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        void WasCalled(Expression<Action<TFake>> callSpecification, Expression<Func<int, bool>> repeatValidation);

        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <typeparam name="TMember">The type of return values from the function that is asserted upon.</typeparam>
        /// <param name="callSpecification">An expression describing the call to assert that has been called.</param>
        void WasCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification);

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        void WasCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification, Expression<Func<int, bool>> repeatValidation);

        /// <summary>
        /// Asserts that the specified call was not made within the current scope.
        /// </summary>
        /// <param name="callSpecification">The call that should not have been made.</param>
        void WasNotCalled(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Asserts that the specified call was not made within the current scope.
        /// </summary>
        /// <param name="callSpecification">The call that should not have been made.</param>
        void WasNotCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification);
    }
}
