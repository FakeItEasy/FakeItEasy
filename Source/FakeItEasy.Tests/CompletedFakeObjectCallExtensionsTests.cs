namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class CompletedFakeObjectCallExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Matching_should_pass_call_specification_to_matcher_factory()
        {
            // Arrange
            var factory = A.Fake<IExpressionCallMatcherFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>()));

            Expression<Action<IFoo>> callSpecification = x => x.Bar();
            var calls = this.CreateFakeCallCollection(callSpecification);

            // Act
            using (Fake.CreateScope())
            {
                this.StubResolve<IExpressionCallMatcherFactory>(factory);
                calls.Matching(callSpecification);
            }

            // Assert
            A.CallTo(() => factory.CreateCallMathcer(callSpecification)).MustHaveHappened();
        }

        [Test]
        public void Matching_should_return_calls_that_are_matched_by_matcher()
        {
            // Arrange
            var calls = this.CreateFakeCallCollection<IFoo>(x => x.Bar(), x => x.Baz(), x => x.Biz());

            var matcher = A.Fake<ICallMatcher>();

            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>._)).Returns(false);
            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>.That.Matches(x => x.Method.Name == "Baz", "Method named Baz"))).Returns(true);
            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>.That.Matches(x => x.Method.Name == "Biz", "Method named Biz"))).Returns(true);

            var factory = A.Fake<IExpressionCallMatcherFactory>();
            A.CallTo(() => factory.CreateCallMathcer(A<LambdaExpression>._)).Returns(matcher);

            // Act
            IEnumerable<ICompletedFakeObjectCall> matchingCalls = null;

            using (Fake.CreateScope())
            {
                this.StubResolve<IExpressionCallMatcherFactory>(factory);
                matchingCalls = calls.Matching<IFoo>(x => x.Bar());
            }

            // Assert
            matchingCalls.Should().HaveCount(2).And
                                  .Contain(x => x.Method.Name == "Baz").And
                                  .Contain(x => x.Method.Name == "Biz");
        }

        private IEnumerable<ICompletedFakeObjectCall> CreateFakeCallCollection<TFake>(params Expression<Action<TFake>>[] callSpecifications)
        {
            return callSpecifications.Select(x => ExpressionHelper.CreateFakeCall<TFake>(x).AsReadOnly());
        }
    }
}
