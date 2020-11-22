namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeManagerTests
    {
        private static readonly IFakeObjectCallRule NonApplicableInterception = new FakeCallRule { IsApplicableTo = x => false };
        private readonly List<object> createdFakes;

        public FakeManagerTests()
        {
            this.createdFakes = new List<object>();
        }

        [Fact]
        public void Event_listeners_that_are_removed_should_not_be_invoked_when_event_is_raised()
        {
            var foo = A.Fake<IFoo>();
            var called = false;
            EventHandler listener = (s, e) => called = true;

            foo.SomethingHappened += listener;
            foo.SomethingHappened -= listener;

            foo.SomethingHappened += Raise.With(EventArgs.Empty);

            called.Should().BeFalse();
        }

        [Fact]
        public void Method_call_should_return_default_value_when_there_is_no_matching_interception_and_return_type_is_value_type()
        {
            var fake = this.CreateFakeManager<IFoo>();
            var result = ((IFoo)fake.Object!).Baz();

            result.Should().Be(0);
        }

        [Fact]
        public void Method_call_should_not_set_return_value_when_there_is_no_matching_interception_and_return_type_is_void()
        {
            var fake = this.CreateFakeManager<IFoo>();
            ((IFoo)fake.Object!).Bar();
        }

        [Fact]
        public void The_first_interceptor_should_be_applied_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRuleFirst(interception);

            // Act
            ((IFoo)fake.Object!).Bar();

            interception.ApplyWasCalled.Should().BeTrue();
        }

        [Fact]
        public void The_first_applicable_interceptor_should_be_called_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRuleFirst(NonApplicableInterception);
            fake.AddRuleFirst(interception);

            ((IFoo)fake.Object!).Bar();

            interception.ApplyWasCalled.Should().BeTrue();
        }

        [Fact]
        public void The_latest_added_rule_should_be_called_forever_when_no_number_of_times_is_specified()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var firstRule = CreateApplicableInterception();
            var latestRule = CreateApplicableInterception();

            fake.AddRuleFirst(firstRule);
            fake.AddRuleFirst(latestRule);

            var foo = (IFoo)fake.Object!;
            foo.Bar();
            foo.Bar();
            foo.Bar();

            firstRule.ApplyWasCalled.Should().BeFalse();
        }

        [Fact]
        public void An_applicable_action_should_be_called_its_specified_number_of_times_before_the_next_applicable_action_is_called()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var applicableTwice = new FakeCallRule
            {
                IsApplicableTo = x => true,
                NumberOfTimesToCall = 2
            };

            var nextApplicable = CreateApplicableInterception();

            fake.AddRuleFirst(nextApplicable);
            fake.AddRuleFirst(applicableTwice);

            ((IFoo)fake.Object!).Bar();
            ((IFoo)fake.Object).Bar();
            nextApplicable.ApplyWasCalled.Should().BeFalse();

            ((IFoo)fake.Object).Bar();
            nextApplicable.ApplyWasCalled.Should().BeTrue();
        }

        [Fact]
        public void DefaultValue_should_be_returned_when_the_last_applicable_action_has_been_used_its_specified_number_of_times()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var applicableTwice = CreateApplicableInterception();
            applicableTwice.NumberOfTimesToCall = 2;
            applicableTwice.Apply = x => x.SetReturnValue(10);

            fake.AddRuleFirst(applicableTwice);

            ((IFoo)fake.Object!).Baz();
            ((IFoo)fake.Object).Baz();

            var result = ((IFoo)fake.Object).Baz();

            result.Should().Be(0);
        }

        [Fact]
        public void Interceptions_should_return_interceptions_that_are_added()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var one = CreateApplicableInterception();
            var two = CreateApplicableInterception();

            fake.AddRuleFirst(one);
            fake.AddRuleFirst(two);

            fake.Rules.Should().BeEquivalentTo(one, two);
        }

        [Fact]
        public void RecordedCalls_contains_all_calls_made_on_the_fake()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var foo = (IFoo)fake.Object!;
            foo.Bar();
            Record.Exception(() => foo[1]);

            fake.GetRecordedCalls().Should().Contain(x => x.Method.Name == "Bar");
            fake.GetRecordedCalls().Should().Contain(x => x.Method.Name == "get_Item");
        }

        [Fact]
        public void RecordedCalls_should_contain_calls_that_throws_exceptions()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake);
            A.CallTo(() => fake.Bar()).Throws(new InvalidOperationException());

            // Act
            Record.Exception(() => fake.Bar());

            // Assert
            manager.GetRecordedCalls().Count().Should().Be(1);
        }

        [Fact]
        public void Object_properties_has_property_behavior_when_not_configured()
        {
            var foo = A.Fake<IFoo>();

            foo.SomeProperty = 10;
            foo.SomeProperty.Should().Be(10);

            foo.SomeProperty = 5;
            foo.SomeProperty.Should().Be(5);
        }

        [Fact]
        public void Object_properties_do_not_have_property_behavior_when_explicitly_configured()
        {
            var foo = A.Fake<IFoo>();

            A.CallTo(() => foo.SomeProperty).Returns(2);
            foo.SomeProperty.Should().Be(2);

            foo.SomeProperty = 5;
            foo.SomeProperty.Should().Be(2);

            A.CallTo(() => foo.SomeProperty).Returns(20);
            foo.SomeProperty.Should().Be(20);

            foo.SomeProperty = 10;
            foo.SomeProperty.Should().Be(20);
        }

        [Fact]
        public void Properties_should_be_set_to_fake_objects_when_property_type_is_fakeable_and_the_property_is_not_explicitly_set()
        {
            var foo = A.Fake<IFoo>();

            foo.ChildFoo.Should().BeAFake();
        }

        [Fact]
        public void Non_configured_property_should_have_same_fake_instance_when_accessed_twice_when_property_is_internal()
        {
            // Arrange
            var foo = A.Fake<Foo>();

            // Act

            // Assert
            foo.InternalVirtualFakeableProperty.Should().NotBeNull().And.BeSameAs(foo.InternalVirtualFakeableProperty);
        }

        [Fact]
        public void Property_should_return_set_value_when_property_is_internal()
        {
            // Arrange
            var foo = A.Fake<Foo>();
            var value = A.Fake<IFoo>();

            // Act
            foo.InternalVirtualFakeableProperty = value;

            // Assert
            foo.InternalVirtualFakeableProperty.Should().BeSameAs(value);
        }

        [Fact]
        public void RemoveRule_should_remove_the_specified_rule_from_the_fake()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Dummy<IFakeObjectCallRule>();
            fake.AddRuleFirst(rule);

            // Act
            fake.RemoveRule(rule);

            // Assert
            fake.Rules.Should().NotContain(rule);
        }

        [Fact]
        public void RemoveRule_should_do_nothing_when_rule_does_not_exist_in_fake()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Dummy<IFakeObjectCallRule>();

            // Act
            fake.RemoveRule(rule);

            // Assert
            fake.Rules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveRule_should_be_null_guarded()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();

            // Act

            // Assert
            Expression<Action> call = () => fake.RemoveRule(A.Dummy<IFakeObjectCallRule>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AddRuleLast_should_add_rule()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Dummy<IFakeObjectCallRule>();

            // Act
            fake.AddRuleLast(rule);

            // Assert
            fake.Rules.Should().Contain(r => ReferenceEquals(r, rule));
        }

        [Fact]
        public void AddRuleLast_should_add_rule_last_among_the_user_specified_rules()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();

            // Act
            fake.AddRuleFirst(A.Fake<IFakeObjectCallRule>());
            fake.AddRuleLast(rule);

            // Assert
            fake.Rules.Last().Should().BeSameAs(rule);
        }

        [Fact]
        public void Constructor_should_set_fake_type_and_fake_object()
        {
            // Arrange
            var proxy = "some string";

            // Act
            var fakeManager = new FakeManager(typeof(string), proxy, null);

            // Assert
            fakeManager.FakeObjectType.Should().Be(typeof(string));
            fakeManager.Object.Should().BeSameAs(proxy);
        }

        [Fact]
        public void Should_clear_all_added_rules_when_calling_clear_user_rules()
        {
            // Arrange
            var manager = new FakeManager(typeof(int), 0, null);
            manager.AddRuleFirst(A.Dummy<IFakeObjectCallRule>());
            manager.AddRuleLast(A.Dummy<IFakeObjectCallRule>());

            // Act
            manager.ClearUserRules();

            // Assert
            manager.Rules.Should().BeEmpty();
        }

        [Fact]
        public void Should_invoke_listener_when_call_is_intercepted()
        {
            // Arrange
            var interceptedCall = A.Fake<InterceptedFakeObjectCall>();

            var listener = A.Fake<IInterceptionListener>();
            var manager = new FakeManager(typeof(int), 0, null);

            manager.AddInterceptionListener(listener);

            // Act
            ProcessFakeObjectCall(manager, interceptedCall);

            // Assert
            A.CallTo(() => listener.OnBeforeCallIntercepted(interceptedCall)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_listener_after_call_has_been_intercepted()
        {
            // Arrange
            var completedCall = A.Dummy<CompletedFakeObjectCall>();
            var interceptedCall = A.Fake<InterceptedFakeObjectCall>();
            A.CallTo(() => interceptedCall.ToCompletedCall()).Returns(completedCall);

            var listener = A.Fake<IInterceptionListener>();
            var manager = new FakeManager(typeof(int), 0, null);

            manager.AddInterceptionListener(listener);

            // Act
            ProcessFakeObjectCall(manager, interceptedCall);

            // Assert
            A.CallTo(() => listener.OnAfterCallIntercepted(completedCall)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_listener_after_call_has_been_intercepted_when_application_of_rule_throws()
        {
            // Arrange
            var completedCall = A.Dummy<CompletedFakeObjectCall>();
            var interceptedCall = A.Fake<InterceptedFakeObjectCall>();
            A.CallTo(() => interceptedCall.ToCompletedCall()).Returns(completedCall);

            var listener = A.Fake<IInterceptionListener>();
            var manager = new FakeManager(typeof(int), 0, null);

            var ruleThatThrows = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => ruleThatThrows.IsApplicableTo(interceptedCall)).Returns(true);
            A.CallTo(() => ruleThatThrows.Apply(A<IInterceptedFakeObjectCall>._)).Throws(new InvalidOperationException());

            manager.AddRuleFirst(ruleThatThrows);
            manager.AddInterceptionListener(listener);

            // Act
            Record.Exception(() => ProcessFakeObjectCall(manager, interceptedCall));

            // Assert
            A.CallTo(() => listener.OnAfterCallIntercepted(completedCall)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_listeners_in_the_correct_order()
        {
            // Arrange
            var manager = new FakeManager(typeof(int), 0, null);
            var listener1 = A.Fake<IInterceptionListener>();
            var listener2 = A.Fake<IInterceptionListener>();

            // Act
            manager.AddInterceptionListener(listener1);
            manager.AddInterceptionListener(listener2);

            ProcessFakeObjectCall(manager, A.Dummy<InterceptedFakeObjectCall>());

            // Assert
            A.CallTo(() => listener2.OnBeforeCallIntercepted(A<IFakeObjectCall>._)).MustHaveHappened()
                .Then(A.CallTo(() => listener1.OnBeforeCallIntercepted(A<IFakeObjectCall>._)).MustHaveHappened())
                .Then(A.CallTo(() => listener1.OnAfterCallIntercepted(A<ICompletedFakeObjectCall>._)).MustHaveHappened())
                .Then(A.CallTo(() => listener2.OnAfterCallIntercepted(A<ICompletedFakeObjectCall>._)).MustHaveHappened());
        }

        private static FakeCallRule CreateApplicableInterception()
        {
            return new FakeCallRule { IsApplicableTo = x => true };
        }

        private static void ProcessFakeObjectCall(IFakeCallProcessor fakeCallProcessor, InterceptedFakeObjectCall interceptedCall)
        {
            fakeCallProcessor.Process(interceptedCall);
        }

        private FakeManager CreateFakeManager<T>() where T : class
        {
            var result = A.Fake<T>();
            this.MakeSureThatWeakReferenceDoesNotGetCollected(result);
            return Fake.GetFakeManager(result);
        }

        private void MakeSureThatWeakReferenceDoesNotGetCollected<T>(T result) where T : class
        {
            this.createdFakes.Add(result);
        }

        public class Foo
        {
            internal virtual IFoo? InternalVirtualFakeableProperty { get; set; }
        }
    }
}
