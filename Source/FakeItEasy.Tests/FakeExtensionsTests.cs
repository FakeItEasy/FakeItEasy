using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using FakeItEasy.Api;
using System.Linq.Expressions;
using System.Collections.Generic;
using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeExtensionsTests
        : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            repeatConfig.Once();

            A.CallTo(() => repeatConfig.NumberOfTimes(1)).Assert(Happened.Once);
        }

        [Test]
        public void Once_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Once((IRepeatConfiguration)null));
        }

        [Test]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration>();
            
            repeatConfig.Twice();

            A.CallTo(() => repeatConfig.NumberOfTimes(2)).Assert(Happened.Once);
        }

        [Test]
        public void Twice_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Twice((IRepeatConfiguration)null));
        }

        [Test]
        public void ReturnsNull_should_set_call_returns_with_null_on_configuration()
        {
            var config = A.Fake<IReturnValueConfiguration<string>>();
            config.ReturnsNull();

            A.CallTo(() => config.Returns(A<string>.That.IsNull())).Assert(Happened.Once);
        }

        [Test]
        public void ReturnsNull_should_return_configuration_object()
        {
            var config = A.Fake<IReturnValueConfiguration<string>>();
            var returnConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();
            
            Configure.Fake(config).CallsTo(x => x.Returns(A<string>.Ignored)).Returns(returnConfig);

            var returned = config.ReturnsNull();
            var f = new Fake<IFoo>();
            
            Assert.That(returned, Is.SameAs(returnConfig));
        }

        [Test]
        public void ReturnsNull_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.ReturnsNull(A.Fake<IReturnValueConfiguration<string>>()));
        }

        [Test]
        public void WithAnyArguments_with_void_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            var configuration = A.Fake<IArgumentValidationConfiguration<IVoidConfiguration>>();

            FakeExtensions.WithAnyArguments(configuration);

            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            Assert.That(predicate.Invoke(null), Is.True);
        }

        [Test]
        public void WithAnyArguments_with_function_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            var configuration = A.Fake<IArgumentValidationConfiguration<IReturnValueConfiguration<int>>>();

            FakeExtensions.WithAnyArguments(configuration);

            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            Assert.That(predicate.Invoke(null), Is.True);
        }

        [Test]
        public void Matching_should_pass_call_specification_to_matcher_factory()
        {
            // Arrange
            LambdaExpression expressionPassedToFactory = null;

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
            A.CallTo(() => factory.CreateCallMathcer(callSpecification)).Assert(Happened.Once);
        }

        [Test]
        public void Matching_should_return_calls_that_are_matched_by_matcher()
        {
            // Arrange
            var calls = this.CreateFakeCallCollection<IFoo>(x => x.Bar(), x => x.Baz(), x => x.Biz());

            var matcher = A.Fake<ICallMatcher>();

            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>.Ignored.Argument)).Returns(false);
            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>.That.Matches(x => x.Method.Name == "Baz").Argument)).Returns(true);
            A.CallTo(() => matcher.Matches(A<IFakeObjectCall>.That.Matches(x => x.Method.Name == "Biz").Argument)).Returns(true);

            var factory = A.Fake<IExpressionCallMatcherFactory>();
            A.CallTo(() => factory.CreateCallMathcer(A<LambdaExpression>.Ignored)).Returns(matcher);

            // Act
            IEnumerable<ICompletedFakeObjectCall> matchingCalls = null;

            using (Fake.CreateScope())
            {
                this.StubResolve<IExpressionCallMatcherFactory>(factory);
                matchingCalls = calls.Matching<IFoo>(x => x.Bar());
            }

            // Assert
            Assert.That(matchingCalls.Count(), Is.EqualTo(2));
            Assert.That(matchingCalls.Any(x => x.Method.Name == "Baz"));
            Assert.That(matchingCalls.Any(x => x.Method.Name == "Biz"));
        }

        private IEnumerable<ICompletedFakeObjectCall> CreateFakeCallCollection<TFake>(params Expression<Action<TFake>>[] callSpecifications)
        {
            return callSpecifications.Select(x => ExpressionHelper.CreateFakeCall<TFake>(x).AsReadOnly());
        }
    }
}
