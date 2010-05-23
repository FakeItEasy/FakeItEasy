using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using FakeItEasy.Core;
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

            A.CallTo(() => repeatConfig.NumberOfTimes(1)).MustHaveHappened(Repeated.Once);
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

            A.CallTo(() => repeatConfig.NumberOfTimes(2)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void Twice_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Twice((IRepeatConfiguration)null));
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
            A.CallTo(() => factory.CreateCallMathcer(callSpecification)).MustHaveHappened(Repeated.Once);
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

        [Test]
        public void MustHaveHappened_should_call_configuration_with_repeat_once()
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustHaveHappened();

            // Assert
            A.CallTo(() => configuration.MustHaveHappened(A<Repeated>.That.Matches(x => x.Matches(1)))).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void MustHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act
            
            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustHaveHappened());
        }

        [TestCase(0, Result = true)]
        [TestCase(1, Result = false)]
        [TestCase(3, Result = false)]
        public bool MustNotHaveHappened_should_call_configuration_with_repeat_that_validates_correctly(int repeat)
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustNotHaveHappened();

            // Assert
            var specifiedRepeat = Fake.GetCalls(configuration).Single().Arguments.Get<Repeated>(0);
            return specifiedRepeat.Matches(repeat);
        }

        [Test]
        public void MustNotHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustNotHaveHappened());
        }

        [Test]
        public void ReturnsNextFromSequence_should_call_returns_with_factory_that_returns_next_from_sequence_for_each_call()
        {
            // Arrange
            var sequence = new[] { 1, 2, 3 };
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var call = A.Fake<IFakeObjectCall>();

            // Act
            FakeExtensions.ReturnsNextFromSequence(config, sequence);

            // Assert
            var factoryValidator = A<Func<IFakeObjectCall, int>>.That.Matches(x => 
            {
                var producedSequence = new[] { x.Invoke(call), x.Invoke(call), x.Invoke(call) };
                return producedSequence.SequenceEqual(sequence);
            });
            
            A.CallTo(() => config.ReturnsLazily(factoryValidator)).MustHaveHappened();
        }

        [Test]
        public void ReturnsNextFromSequence_should_return_repeat_config_from_passed_in_configuration()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var returnedConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();

            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>.Ignored)).Returns(returnedConfig);

            // Act

            // Assert
            Assert.That(FakeExtensions.ReturnsNextFromSequence(config, 1), Is.SameAs(returnedConfig));
        }

        [Test]
        public void ReturnsNextFromSequence_should_return_null_when_all_values_has_been_returned()
        {
            // Arrange
            var sequence = new[] { "" };
            var config = A.Fake<IReturnValueConfiguration<string>>();

            // Act
            FakeExtensions.ReturnsNextFromSequence(config, sequence);

            // Assert
            var factoryValidator = A<Func<IFakeObjectCall, string>>.That.Matches(x =>
            {
                x.Invoke(A.Dummy<IFakeObjectCall>());
                return x.Invoke(A.Dummy<IFakeObjectCall>()) == null;
            });

            A.CallTo(() => config.ReturnsLazily(factoryValidator)).MustHaveHappened();
        }

        [Test]
        public void Returns_should_return_configuration_returned_from_passed_in_configuration()
        {
            // Arrange
            var expectedConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();
            var config = A.Fake <IReturnValueConfiguration<int>>();
            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>.That.Matches(x => x.Invoke(null) == 10))).Returns(expectedConfig);
            
            // Act
            var returned = FakeExtensions.Returns(config, 10);

            // Assert
            Assert.That(returned, Is.SameAs(expectedConfig));
        }

        private IEnumerable<ICompletedFakeObjectCall> CreateFakeCallCollection<TFake>(params Expression<Action<TFake>>[] callSpecifications)
        {
            return callSpecifications.Select(x => ExpressionHelper.CreateFakeCall<TFake>(x).AsReadOnly());
        }
    }
}

