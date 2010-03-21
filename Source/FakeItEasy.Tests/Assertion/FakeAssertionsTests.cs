using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using FakeItEasy.Assertion;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.Assertion
{
    [TestFixture]
    public class FakeAssertionsTests
    {
        private FakeObject fake;
        private FakeAssertions<IFoo> assertions;
        private IExpressionCallMatcherFactory callMatcherFactory;
        private FakeAsserter fakeAsserter;
        private FakeAsserter.Factory fakeAsserterFactory;
        private IEnumerable<IFakeObjectCall> argumentToFakeAsserterFactory;
        private ExpressionCallMatcher matcher;

        [SetUp]
        public void SetUp()
        {
            this.fake = Fake.GetFakeObject(ServiceLocator.Current.Resolve<FakeObjectFactory>().CreateFake(typeof(IFoo), null, false));
            
            this.fakeAsserter = A.Fake<FakeAsserter>();
            
            this.matcher = A.Fake<ExpressionCallMatcher>(x => x.WithArgumentsForConstructor(() =>
                new ExpressionCallMatcher(
                    ExpressionHelper.CreateExpression<IFoo>(_ => Console.WriteLine("")),
                    ServiceLocator.Current.Resolve<ArgumentConstraintFactory>(),
                    ServiceLocator.Current.Resolve<MethodInfoManager>())));

            this.callMatcherFactory = A.Fake<IExpressionCallMatcherFactory>();
            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(A<LambdaExpression>.Ignored)).Returns(() => this.matcher);

            this.fakeAsserterFactory = x =>
                {
                    this.argumentToFakeAsserterFactory = x;
                    return this.fakeAsserter;
                };

            this.assertions = new FakeAssertions<IFoo>(this.fake, this.callMatcherFactory, this.fakeAsserterFactory);

            this.FakedFoo.Bar();
            this.FakedFoo.Baz();
        }

        [TearDown]
        public void TearDown()
        {
            this.argumentToFakeAsserterFactory = null;
        }

        private IFoo FakedFoo
        {
            get
            {
                return (IFoo)this.fake.Object;
            }
        }

        // Void call no repeat
        [Test]
        public void WasCalled_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasCalled(x => x.Bar());

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasCalled_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
            this.assertions.WasCalled(expression);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasCalled_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasCalled(x => x.Bar());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasCalled_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasCalled(x => x.Bar());

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, true)]
        public void WasCalled_should_call_asserter_with_repeat_predicate_that_validates_values_above_zero(int value, bool result)
        {
            this.assertions.WasCalled(x => x.Bar());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate(value), Is.EqualTo(result));
        }

        [Test]
        public void WasCalled_should_call_asserter_with_more_than_once_as_repeat_description()
        {
            this.assertions.WasCalled(x => x.Bar());

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("at least once"));
        }

        // Function call no repeat
        [Test]
        public void WasCalled_for_function_call_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasCalled(x => x.Biz());

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasCalled_for_function_call_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Biz());
            this.assertions.WasCalled(expression);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasCalled_for_function_call_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasCalled(x => x.Biz());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasCalled_for_function_call_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasCalled(x => x.Biz());

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, true)]
        public void WasCalled_for_function_call_should_call_asserter_with_repeat_predicate_that_validates_values_above_zero(int value, bool result)
        {
            this.assertions.WasCalled(x => x.Biz());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate(value), Is.EqualTo(result));
        }

        [Test]
        public void WasCalled_for_function_call_should_call_asserter_with_more_than_once_as_repeat_description()
        {
            this.assertions.WasCalled(x => x.Biz());

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("at least once"));
        }

        // Void call with repeat
        [Test]
        public void WasCalled_for_void_call_with_repeat_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasCalled(x => x.Bar(), repeat => repeat == 10);

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasCalled_for_void_call_with_repeat_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
            this.assertions.WasCalled(expression, repeat => repeat == 10);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasCalled_for_void_call_with_repeat_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasCalled(x => x.Bar(), repeat => repeat == 10);

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasCalled_for_void_call_with_repeat_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasCalled(x => x.Bar(), repeat => repeat == 10);

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        public void WasCalled_for_void_call_with_repeat_should_call_asserter_with_compiled_expression()
        {
            var predicateExpression = ExpressionHelper.CreateExpression<int, bool>(x => x == 10);
            this.assertions.WasCalled(x => x.Bar(), predicateExpression );

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate, Is.EqualTo(predicateExpression.Compile()));
        }

        [Test]
        public void WasCalled_for_void_call_with_repeat_should_call_asserter_with_more_than_once_as_repeat_description()
        {
            this.assertions.WasCalled(x => x.Bar(), repeat => repeat > 10);

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("the number of times specified by the lambda repeat => (repeat > 10)"));
        }

        // Function call with repeat
        [Test]
        public void WasCalled_for_function_call_with_repeat_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasCalled(x => x.Biz(), repeat => repeat == 10);

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasCalled_for_function_call_with_repeat_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Biz());
            this.assertions.WasCalled(expression, repeat => repeat == 10);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasCalled_for_function_call_with_repeat_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasCalled(x => x.Biz(), repeat => repeat == 10);

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasCalled_for_function_call_with_repeat_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasCalled(x => x.Biz(), repeat => repeat == 10);

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        public void WasCalled_for_function_call_with_repeat_should_call_asserter_with_compiled_expression()
        {
            var predicateExpression = ExpressionHelper.CreateExpression<int, bool>(x => x == 10);
            this.assertions.WasCalled(x => x.Biz(), predicateExpression);

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate, Is.EqualTo(predicateExpression.Compile()));
        }

        [Test]
        public void WasCalled_for_function_call_with_repeat_should_call_asserter_with_more_than_once_as_repeat_description()
        {
            this.assertions.WasCalled(x => x.Biz(), repeat => repeat > 10);

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("the number of times specified by the lambda repeat => (repeat > 10)"));
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new FakeAssertions<IFoo>(this.fake, this.callMatcherFactory, x => this.fakeAsserter));
        }

        // Was not called with void call
        [Test]
        public void WasNotCalled_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasNotCalled(x => x.Bar());

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasNotCalled_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
            this.assertions.WasNotCalled(expression);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasNotCalled_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasNotCalled(x => x.Bar());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasNotCalled_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasNotCalled(x => x.Bar());

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(10, false)]
        public void WasNotCalled_should_call_asserter_with_repeat_predicate_that_validates_zero_only(int value, bool result)
        {
            this.assertions.WasNotCalled(x => x.Bar());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate(value), Is.EqualTo(result));
        }

        [Test]
        public void WasNotCalled_should_call_asserter_with_never_as_repeat_description()
        {
            this.assertions.WasNotCalled(x => x.Bar());

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("never"));
        }

        // Was not called with function call
        [Test]
        public void WasNotCalled_with_function_call_should_call_asserter_factory_with_call_collection_of_fake()
        {
            this.assertions.WasNotCalled(x => x.Biz());

            Assert.That(this.argumentToFakeAsserterFactory, Is.EquivalentTo(this.fake.RecordedCallsInScope));
        }

        [Test]
        public void WasNotCalled_with_function_call_should_call_matcher_factory_with_expression()
        {
            var expression = ExpressionHelper.CreateExpression<IFoo>(x => x.Biz());
            this.assertions.WasNotCalled(expression);

            A.CallTo(() => this.callMatcherFactory.CreateCallMathcer(expression)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void WasNotCalled_with_function_call_should_call_asserter_with_matcher_Matches_delegate()
        {
            this.assertions.WasNotCalled(x => x.Biz());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            Assert.That(predicate.Target, Is.SameAs(this.matcher));
        }

        [Test]
        public void WasNotCalled_with_function_call_should_call_asserter_with_matcher_ToString_as_call_description()
        {
            A.CallTo(() => this.matcher.ToString()).Returns("description");

            this.assertions.WasNotCalled(x => x.Biz());

            string description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("callDescription");

            Assert.That(description, Is.EqualTo("description"));
        }

        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(10, false)]
        public void WasNotCalled_with_function_call_should_call_asserter_with_repeat_predicate_that_validates_zero_only(int value, bool result)
        {
            this.assertions.WasNotCalled(x => x.Biz());

            var predicate = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(predicate(value), Is.EqualTo(result));
        }

        [Test]
        public void WasNotCalled_with_function_call_should_call_asserter_with_never_as_repeat_description()
        {
            this.assertions.WasNotCalled(x => x.Biz());

            var description = Fake.GetCalls(this.fakeAsserter).Single().Arguments.Get<string>("repeatDescription");

            Assert.That(description, Is.EqualTo("never"));
        }

        //[Test]
        //public void WasCalled_with_void_call_should_call_asserter_with_call_matcher()
        //{
        //    this.assertions.WasCalled(x => x.Bar());

        //    var callPredicate = Fake.GetCalls(this.fakeAsserter).First().Arguments.Get<Func<IFakeObjectCall, bool>>(0);

        //    using (Fake.CreateScope())
        //    {
        //        callPredicate.Invoke(A.Fake<IFakeObjectCall>());
                
        //        Assert.That(Fake.GetCalls(this.matcher).Matching(x => x.Matches(Argument.Is.Any<IFakeObjectCall>())).Count(), Is.EqualTo(1));
        //    }
        //}

        //[Test]
        //public void WasCalled_with_void_call_should_call_asserter_with_correct_call_description()
        //{
        //    // Arrange
        //    Configure.Fake(this.matcher).CallsTo(x => x.ToString()).Returns("description");
            
        //    // Act
        //    this.assertions.WasCalled(x => x.Bar("test", 1));
            
        //    // Assert
        //    var description = Fake.GetCalls(this.fakeAsserter).First().Arguments.Get<string>(1);
        //    Assert.That(description, Is.EqualTo("description"));
        //}

        //private Asserter
    }
}
