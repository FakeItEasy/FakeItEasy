namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class ExpressionCallMatcherTests
    {
        private ArgumentConstraintFactory constraintFactory;
        private MethodInfoManager methodInfoManager;

        [SetUp]
        public void SetUp()
        {
            this.constraintFactory = A.Fake<ArgumentConstraintFactory>();
            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.IsValid(A<object>.Ignored)).Returns(true);
            A.CallTo(() => constraintFactory.GetArgumentConstraint(A<Expression>.Ignored)).Returns(validator);

            this.methodInfoManager = A.Fake<MethodInfoManager>();
        }

        private ExpressionCallMatcher CreateMatcher<TFake, TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            return CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher(LambdaExpression callSpecification)
        {
            return new ExpressionCallMatcher(callSpecification, this.constraintFactory, this.methodInfoManager);
        }

        private void StubMethodInfoManagerToReturn(bool returnValue)
        {
            A.CallTo(() => this.methodInfoManager.WillInvokeSameMethodOnTarget(A<Type>.Ignored, A<MethodInfo>.Ignored, A<MethodInfo>.Ignored))
                .Returns(returnValue);
        }

        [Test]
        public void Constructor_should_throw_if_expression_is_not_property_or_method()
        {
            Assert.Throws<ArgumentException>(() =>
                this.CreateMatcher<Foo, IServiceProvider>(x => x.ServiceProvider));
        }

        [Test]
        public void Matches_should_return_true_when_MethodInfoManager_returns_true()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            this.StubMethodInfoManagerToReturn(true);
            
            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_should_return_false_when_MethodInfoManager_returns_false()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            this.StubMethodInfoManagerToReturn(false);

            Assert.That(matcher.Matches(call), Is.False);
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

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.That.ProducesValue("foo"))).MustHaveHappened();
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.That.ProducesValue(10))).MustHaveHappened();
        }

        [Test]
        public void Matches_should_use_argument_validators_to_validate_each_argument_of_call()
        {
            this.StubMethodInfoManagerToReturn(true);

            var argument1 = "foo";
            var argument2 = 10;

            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.IsValid(A<object>.Ignored))
                .Returns(true);
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.Ignored))
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
            A.CallTo(() => validator.IsValid(A<object>.Ignored)).Returns(false);
            A.CallTo(() => validator.IsValid(A<object>.Ignored)).Returns(true).Once();

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.Ignored)).Returns(validator);
            
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1, 2));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 3));

            Assert.That(matcher.Matches(call), Is.False);
        }

        [Test]
        public void ToString_should_write_full_method_name_with_type_name_and_arguments_list()
        {
            var validator = A.Fake<IArgumentConstraint>();
            A.CallTo(() => validator.ToString()).Returns("<FOO>");

            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.Ignored)).Returns(validator);

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 2));

            Assert.That(matcher.ToString(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<FOO>, <FOO>)"));
            
            A.CallTo(() => this.constraintFactory.GetArgumentConstraint(A<Expression>.Ignored)).MustHaveHappened(Repeated.Twice.Exactly);
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

            Assert.That(argumentsPassedToPredicate.AsEnumerable(), Is.EquivalentTo(new object[] { 1, 2 }));
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

            Assert.That(matcher.ToString(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<Predicated>, <Predicated>)"));
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
    }

    public abstract class TypeWithInternalProperty
    {
        internal abstract bool InternalProperty { get; }
    }
}