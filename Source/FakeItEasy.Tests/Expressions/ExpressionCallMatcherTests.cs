namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class ExpressionCallMatcherTests
    {
        private ExpressionArgumentConstraintFactory constraintFactory;
        private MethodInfoManager methodInfoManager;
        private CallExpressionParser parser;

        [SetUp]
        public void Setup()
        {
            this.constraintFactory = A.Fake<ExpressionArgumentConstraintFactory>();
            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.IsValid(A<object>._)).Returns(true);
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>._)).Returns(validator);

            this.methodInfoManager = A.Fake<MethodInfoManager>();
            this.parser = new CallExpressionParser();
        }

        [Test]
        public void Constructor_should_throw_if_expression_is_not_property_or_method()
        {
            var exception = Record.Exception(() =>
                this.CreateMatcher<Foo, IServiceProvider>(x => x.ServiceProvider));

            exception.Should().BeAnExceptionOfType<ArgumentException>();
        }

        [Test]
        public void Matches_should_return_true_when_MethodInfoManager_returns_true()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            this.StubMethodInfoManagerToReturn(true);
            
            matcher.Matches(call).Should().BeTrue();
        }

        [Test]
        public void Matches_should_return_false_when_MethodInfoManager_returns_false()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            this.StubMethodInfoManagerToReturn(false);

            matcher.Matches(call).Should().BeFalse();
        }

        [Test]
        public void Matches_should_call_MethodInfoManager_with_method_from_call_and_method_from_expression()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
            var expressionMethod = ((MethodCallExpression)expression.Body).Method;

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            matcher.Matches(call);

            A.CallTo(() => this.methodInfoManager.WillInvokeSameMethodOnTarget(call.FakedObject.GetType(), call.Method, expressionMethod)).MustHaveHappened();
        }

        [Test]
        public void Matches_should_call_MethodInfoManager_with_property_getter_method_when_call_is_property_access()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo, int>(x => x.SomeProperty);
            var matcher = this.CreateMatcher<IFoo, int>(x => x.SomeProperty);

            var getter = typeof(IFoo).GetProperty("SomeProperty").GetGetMethod();

            matcher.Matches(call);

            A.CallTo(() => this.methodInfoManager.WillInvokeSameMethodOnTarget(call.FakedObject.GetType(), getter, getter)).MustHaveHappened();
        }

        [Test]
        public void Matches_should_use_ArgumentValidatorManager_to_create_validator_for_each_argument()
        {
            this.CreateMatcher<IFoo>(x => x.Bar("foo", 10));

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>.That.ProducesValue("foo"))).MustHaveHappened();
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>.That.ProducesValue(10))).MustHaveHappened();
        }

        [Test]
        public void Matches_should_use_argument_validators_to_validate_each_argument_of_call()
        {
            this.StubMethodInfoManagerToReturn(true);

            var argument1 = "foo";
            var argument2 = 10;

            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.IsValid(A<object>._))
                .Returns(true);
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>._))
                .Returns(validator);
            
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(argument1, argument2));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(argument1, argument2));

            matcher.Matches(call);

            A.CallTo(() => validator.IsValid(argument1)).MustHaveHappened();
            A.CallTo(() => validator.IsValid(argument2)).MustHaveHappened();
        }

        [Test]
        public void Matches_should_return_false_when_any_argument_validator_returns_false()
        {
            this.StubMethodInfoManagerToReturn(true);

            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.IsValid(A<object>._)).Returns(false);
            A.CallTo(() => validator.IsValid(A<object>._)).Returns(true).Once();

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>._)).Returns(validator);
            
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1, 2));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 3));

            matcher.Matches(call).Should().BeFalse();
        }

        [Test]
        public void ToString_should_write_full_method_name_with_type_name_and_arguments_list()
        {
            var constraint = A.Fake<IArgumentConstraint>();
            A.CallTo(() => constraint.WriteDescription(A<IOutputWriter>._))
                .Invokes(x => x.GetArgument<IOutputWriter>(0).Write("<FOO>"));

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>._)).Returns(constraint);

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 2));

            matcher.ToString().Should().Be("FakeItEasy.Tests.IFoo.Bar(<FOO>, <FOO>)");

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<ParsedArgumentExpression>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void UsePredicateToValidateArguments_should_configure_matcher_to_pass_arguments_to_the_specified_predicate()
        {
            this.StubMethodInfoManagerToReturn(true);
            ArgumentCollection argumentsPassedToPredicate = null;

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(null, null));
            
            matcher.UsePredicateToValidateArguments(x =>
                {
                    argumentsPassedToPredicate = x;
                    return true;
                });

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1, 2));
            matcher.Matches(call);

            argumentsPassedToPredicate.Should().BeEquivalentTo(new object[] { 1, 2 });
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool UsePredicateToValidateArguments_should_configure_matcher_to_return_predicate_result_when_method_matches(bool predicateReturnValue)
        {
            this.StubMethodInfoManagerToReturn(true);

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(null, null));
            matcher.UsePredicateToValidateArguments(x => predicateReturnValue);

            return matcher.Matches(ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1, 2)));
        }

        [Test]
        public void ToString_should_write_predicate_when_predicate_is_used_to_validate_arguments()
        {
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(null, null));

            matcher.UsePredicateToValidateArguments(x => true);

            matcher.ToString().Should().Be("FakeItEasy.Tests.IFoo.Bar(<Predicated>, <Predicated>)");
        }

        [Test]
        public void Matches_should_call_MethodInfoManager_with_property_getter_method_when_call_is_internal_property_access()
        {
            var call = ExpressionHelper.CreateFakeCall<TypeWithInternalProperty, bool>(x => x.InternalProperty);
            var matcher = this.CreateMatcher<TypeWithInternalProperty, bool>(x => x.InternalProperty);

            var getter = typeof(TypeWithInternalProperty).GetProperty("InternalProperty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true);

            matcher.Matches(call);

            A.CallTo(() => this.methodInfoManager.WillInvokeSameMethodOnTarget(call.FakedObject.GetType(), getter, getter)).MustHaveHappened();
        }

        private ExpressionCallMatcher CreateMatcher<TFake, TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            return this.CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return this.CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher(LambdaExpression callSpecification)
        {
            return new ExpressionCallMatcher(callSpecification, this.constraintFactory, this.methodInfoManager, this.parser);
        }

        private void StubMethodInfoManagerToReturn(bool returnValue)
        {
            A.CallTo(() => this.methodInfoManager.WillInvokeSameMethodOnTarget(A<Type>._, A<MethodInfo>._, A<MethodInfo>._))
                .Returns(returnValue);
        }
    }
}