namespace FakeItEasy.Assertion
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides assertions for fake objects.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    internal class FakeAssertions<TFake> 
        : IFakeAssertions<TFake>
    {
        /// <summary>
        /// The injected call matcher factory.
        /// </summary>
        private IExpressionCallMatcherFactory callMatcherFactory;

        /// <summary>
        /// The injected fake asserter factory.
        /// </summary>
        private FakeAsserter.Factory fakeAsserterFactory;

        /// <summary>
        /// The fake to do assertions for.
        /// </summary>
        private FakeObject fake;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeAssertions&lt;TFake&gt;"/> class.
        /// </summary>
        /// <param name="fake">The fake.</param>
        /// <param name="callMatcherFactory">The call matcher factory.</param>
        /// <param name="fakeAsserterFactory">The fake asserter factory.</param>
        public FakeAssertions(FakeObject fake, IExpressionCallMatcherFactory callMatcherFactory, FakeAsserter.Factory fakeAsserterFactory)
        {
            Guard.AgainstNull(fake, "fake");
            Guard.AgainstNull(callMatcherFactory, "callMatcherFactory");
            Guard.AgainstNull(fakeAsserterFactory, "fakeAsserterFactory");

            this.callMatcherFactory = callMatcherFactory;
            this.fakeAsserterFactory = fakeAsserterFactory;
            this.fake = fake;
        }

        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <param name="callSpecification"></param>
        public void WasCalled(Expression<Action<TFake>> callSpecification)
        {
            this.DoAssertion(callSpecification, repeat => repeat > 0, "at least once");
        }

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        public void WasCalled(Expression<Action<TFake>> callSpecification, Expression<Func<int, bool>> repeatValidation)
        {
            this.DoAssertion(callSpecification, repeatValidation.Compile(), GetRepeatDescriptionFromExpression(repeatValidation));
        }

        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <typeparam name="TMember">The type of return values from the function that is asserted upon.</typeparam>
        /// <param name="callSpecification">An expression describing the call to assert that has been called.</param>
        public void WasCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            this.DoAssertion(callSpecification, repeat => repeat > 0, "at least once");
        }

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="callSpecification">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        public void WasCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification, Expression<Func<int, bool>> repeatValidation)
        {
            this.DoAssertion(callSpecification, repeatValidation.Compile(), GetRepeatDescriptionFromExpression(repeatValidation));
        }

        private static string GetRepeatDescriptionFromExpression(LambdaExpression repeatExpression)
        {
            return "the number of times specified by the lambda {0}".FormatInvariant(repeatExpression.ToString());
        }

        /// <summary>
        /// Asserts that the specified call was not made within the current scope.
        /// </summary>
        /// <param name="callSpecification">The call that should not have been made.</param>
        public void WasNotCalled(Expression<Action<TFake>> callSpecification)
        {
            this.DoAssertion(callSpecification, repeat => repeat == 0, "never");
        }

        /// <summary>
        /// Asserts that the specified call was not made within the current scope.
        /// </summary>
        /// <param name="callSpecification">The call that should not have been made.</param>
        public void WasNotCalled<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            this.DoAssertion(callSpecification, repeat => repeat == 0, "never");
        }

        private void DoAssertion(LambdaExpression callSpecification, Func<int, bool> repeatValidation, string repeatDescription)
        {
            var matcher = this.callMatcherFactory.CreateCallMathcer(callSpecification);
            var asserter = this.fakeAsserterFactory(this.fake.RecordedCallsInScope.Cast<IFakeObjectCall>());
            asserter.AssertWasCalled(matcher.Matches, matcher.ToString(), repeatValidation, repeatDescription);
        }
    }
}