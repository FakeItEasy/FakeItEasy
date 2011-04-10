using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    using FakeItEasy.Creation;

    [TestFixture]
    public class FakeExtensionsTests
        : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            repeatConfig.Once();

            A.CallTo(() => repeatConfig.NumberOfTimes(1)).MustHaveHappened();
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
            
            A.CallTo(() => repeatConfig.NumberOfTimes(2)).MustHaveHappened();
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
            A.CallTo(() => configuration.MustHaveHappened(A<Repeated>.That.Matches(x => x.Matches(1)))).MustHaveHappened();
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
            Func<Func<IFakeObjectCall, int>> factoryValidator = () => A<Func<IFakeObjectCall, int>>.That.Matches(x => 
            {
                var producedSequence = new[] { x.Invoke(call), x.Invoke(call), x.Invoke(call) };
                return producedSequence.SequenceEqual(sequence);
            }, "Predicate");
            
            A.CallTo(() => config.ReturnsLazily(factoryValidator.Invoke())).MustHaveHappened();
        }

        [Test]
        public void ReturnsNextFromSequence_should_set_repeat_to_the_number_of_values_in_sequence()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var returnedConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();
            
            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>._)).Returns(returnedConfig);

            // Act
            FakeExtensions.ReturnsNextFromSequence(config, 1, 2, 3);

            // Assert
            A.CallTo(() => returnedConfig.NumberOfTimes(3)).MustHaveHappened();
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

        [Test]
        public void Returns_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() => 
                A.Fake<IReturnValueConfiguration<string>>().Returns(null));
        }

        [Test]
        public void Curried_ReturnsLazily_returns_value_from_curried_function()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            int currentValue = 10;

            // Act
            config.ReturnsLazily(() => currentValue);

            // Assert
            var curriedFunction = Fake.GetCalls(config).Single().Arguments.Get<Func<IFakeObjectCall, int>>(0);

            Assert.That(curriedFunction.Invoke(A.Dummy<IFakeObjectCall>()), Is.EqualTo(currentValue));
            currentValue = 20;
            Assert.That(curriedFunction.Invoke(A.Dummy<IFakeObjectCall>()), Is.EqualTo(currentValue));
        }

        [Test]
        public void Curried_ReturnsLazily_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IReturnValueConfiguration<int>>().ReturnsLazily(() => 10));
        }

        [Test]
        public void WriteCalls_should_throw_when_calls_is_null()
        {
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.Write(Enumerable.Empty<IFakeObjectCall>(), A.Dummy<IOutputWriter>()));
        }

        [Test]
        public void WriteCalls_should_call_call_writer_registered_in_container_with_calls()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);
            
            var callWriter = A.Fake<CallWriter>();
            this.StubResolve<CallWriter>(callWriter);

            var writer = A.Dummy<IOutputWriter>();

            // Act
            FakeExtensions.Write(calls, writer);

            // Assert
            A.CallTo(() => callWriter.WriteCalls(calls, writer)).MustHaveHappened();
        }

        [Test]
        public void WriteToConsole_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.WriteToConsole(Enumerable.Empty<IFakeObjectCall>()));
        }

        [Test]
        public void WriteToConsole_should_call_writer_registered_in_container_with_calls()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve<CallWriter>(callWriter);

            // Act
            FakeExtensions.WriteToConsole(calls);

            // Assert
            A.CallTo(() => callWriter.WriteCalls(calls, A<IOutputWriter>._)).MustHaveHappened();
        }

        [Test]
        public void WriteToConsole_should_call_writer_registered_in_container_with_console_out()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve<CallWriter>(callWriter);

            // Act
            FakeExtensions.WriteToConsole(calls);

            // Assert
            A.CallTo(() => callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>._, A<IOutputWriter>._)).MustHaveHappened();
        }

        [Test]
        public void GetArgument_delegates_to_the_argument_collections_get_method_when_using_index()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new string[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>(0);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetArgument_delegates_to_the_argument_collections_get_method_when_using_name()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new string[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>("argument2");

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetArgument_should_be_null_guarded_when_using_index()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.GetArgument<int>(A.Fake<IFakeObjectCall>(), 0));
        }

        [Test]
        public void GetArgument_should_be_null_guarded_when_using_argument_name()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.GetArgument<int>(A.Fake<IFakeObjectCall>(), "foo"));
        }

        [Test]
        public void Strict_should_configure_fake_to_throw_expectation_exception()
        {
            // Arrange
            var foo = A.Fake<IFoo>(x => x.Strict());

            // Act

            // Assert
            Assert.That(delegate()
                            {
                                foo.Bar();
                            },
                            Throws.Exception.InstanceOf<ExpectationException>()
                            .With.Message.EqualTo("Call to non configured method \"Bar\" of strict fake."));
        }

        [Test]
        public void Strict_should_return_configuration_object()
        {
            // Arrange
            var options = A.Fake<IFakeOptionsBuilder<IFoo>>();
            Any.CallTo(options).WithReturnType<IFakeOptionsBuilder<IFoo>>().Returns(options);

            // Act
            var result = options.Strict();

            // Assert
            Assert.That(result, Is.SameAs(options));
        }

        [Test]
        public void Strict_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() => 
                FakeExtensions.Strict(A.Dummy<IFakeOptionsBuilder<IFoo>>()));
        }

        [Test]
        public void Where_should_return_configuration_from_configuration()
        {
            // Arrange
            var returnedConfiguration = A.Dummy<IVoidConfiguration>();
            
            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();
            A.CallTo(configuration).WithReturnType<IVoidConfiguration>().Returns(returnedConfiguration);

            // Act

            // Assert
            Assert.That(configuration.Where(x => true), Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void Where_should_pass_writer_that_writes_predicate_as_string()
        {
            // Arrange
            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();
            
            // Act
            configuration.Where(x => true);

            // Assert
            A.CallTo(() => configuration.Where(
                A<Func<IFakeObjectCall, bool>>._, 
                A<Action<IOutputWriter>>.That.Writes("x => True"))).MustHaveHappened();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Where_should_pass_compiled_predicate_to_configuration(bool predicateReturnValue)
        {
            // Arrange
            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();

            // Act
            configuration.Where(x => predicateReturnValue);

            // Assert
            A.CallTo(() => configuration.Where(
                A<Func<IFakeObjectCall, bool>>.That.Returns(A.Dummy<IFakeObjectCall>(), predicateReturnValue),
                A<Action<IOutputWriter>>._)).MustHaveHappened();
        }

        private IEnumerable<ICompletedFakeObjectCall> CreateFakeCallCollection<TFake>(params Expression<Action<TFake>>[] callSpecifications)
        {
            return callSpecifications.Select(x => ExpressionHelper.CreateFakeCall<TFake>(x).AsReadOnly());
        }
    }
}

