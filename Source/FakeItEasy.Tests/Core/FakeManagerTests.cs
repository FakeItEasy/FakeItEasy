namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.ExtensionSyntax;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class FakeManagerTests
    {
        private static readonly IFakeObjectCallRule NonApplicableInterception = new FakeCallRule { IsApplicableTo = x => false };
        private List<object> createdFakes;

        [SetUp]
        public void Setup()
        {
            this.createdFakes = new List<object>();
        }

        [Test]
        public void Calls_configured_in_a_child_context_does_not_exist_outside_that_context()
        {
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();

            using (Fake.CreateScope())
            {
                fake.AddRuleFirst(rule);
            }

            Assert.That(fake.Rules, Has.None.EqualTo(rule));
        }

        [Test]
        public void Event_listeners_that_are_removed_should_not_be_invoked_when_event_is_raised()
        {
            var foo = A.Fake<IFoo>();
            var called = false;
            EventHandler listener = (s, e) => called = true;

            foo.SomethingHappened += listener;
            foo.SomethingHappened -= listener;

            foo.SomethingHappened += Raise.With(EventArgs.Empty);

            Assert.That(called, Is.False);
        }

        [Test]
        public void Method_call_should_return_default_value_when_there_is_no_matching_interception_and_return_type_is_value_type()
        {
            var fake = this.CreateFakeManager<IFoo>();
            var result = ((IFoo)fake.Object).Baz();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Method_call_should_not_set_return_value_when_there_is_no_matching_interception_and_return_type_is_void()
        {
            var fake = this.CreateFakeManager<IFoo>();
            ((IFoo)fake.Object).Bar();
        }

        [Test]
        public void The_first_interceptor_should_be_applied_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRuleFirst(interception);

            // Act
            ((IFoo)fake.Object).Bar();

            Assert.That(interception.ApplyWasCalled, Is.True);
        }

        [Test]
        public void The_first_applicable_interceptor_should_be_called_when_it_has_not_been_used()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var interception = new FakeCallRule
            {
                IsApplicableTo = x => true
            };

            fake.AddRuleFirst(NonApplicableInterception);
            fake.AddRuleFirst(interception);

            ((IFoo)fake.Object).Bar();

            Assert.That(interception.ApplyWasCalled, Is.True);
        }

        [Test]
        public void The_latest_added_rule_should_be_called_forever_when_no_number_of_times_is_specified()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var firstRule = CreateApplicableInterception();
            var latestRule = CreateApplicableInterception();

            fake.AddRuleFirst(firstRule);
            fake.AddRuleFirst(latestRule);

            var foo = (IFoo)fake.Object;
            foo.Bar();
            foo.Bar();
            foo.Bar();

            Assert.That(firstRule.ApplyWasCalled, Is.False);
        }

        [Test]
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

            ((IFoo)fake.Object).Bar();
            ((IFoo)fake.Object).Bar();
            Assert.That(nextApplicable.ApplyWasCalled, Is.False);

            ((IFoo)fake.Object).Bar();
            Assert.That(nextApplicable.ApplyWasCalled, Is.True);
        }

        [Test]
        public void DefaultValue_should_be_returned_when_the_last_applicable_action_has_been_used_its_specified_number_of_times()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var applicableTwice = CreateApplicableInterception();
            applicableTwice.NumberOfTimesToCall = 2;
            applicableTwice.Apply = x => x.SetReturnValue(10);

            fake.AddRuleFirst(applicableTwice);

            ((IFoo)fake.Object).Baz();
            ((IFoo)fake.Object).Baz();

            var result = ((IFoo)fake.Object).Baz();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Interceptions_should_return_interceptions_that_are_added()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var one = CreateApplicableInterception();
            var two = CreateApplicableInterception();

            fake.AddRuleFirst(one);
            fake.AddRuleFirst(two);

            Assert.That(fake.Rules, Is.EquivalentTo(new[] { one, two }));
        }

        [Test]
        public void RecordedCalls_contains_all_calls_made_on_the_fake()
        {
            var fake = this.CreateFakeManager<IFoo>();

            var foo = (IFoo)fake.Object;
            foo.Bar();
            Record.Exception(() => foo[1]);

            Assert.That(fake.RecordedCallsInScope, Has.Some.Matches<IFakeObjectCall>(x => x.Method.Name == "Bar"));
            Assert.That(fake.RecordedCallsInScope, Has.Some.Matches<IFakeObjectCall>(x => x.Method.Name == "get_Item"));
        }

        [Test]
        public void RecordedCalls_should_contain_calls_that_throws_exceptions()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake);
            A.CallTo(() => fake.Bar()).Throws(new InvalidOperationException());

            // Act
            Record.Exception(() => fake.Bar());

            // Assert
            Assert.That(manager.RecordedCallsInScope.Count(), Is.EqualTo(1));
        }

        [Test]
        public void RecordedCalls_only_returns_calls_made_within_the_scope()
        {
            var foo = A.Fake<IFoo>();

            foo.Baz();

            using (Fake.CreateScope())
            {
                foo.Baz();

                Assert.That(Fake.GetCalls(foo).Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void RecordedCalls_returns_calls_made_in_scope_and_any_inner_scopes()
        {
            var foo = A.Fake<IFoo>();

            foo.Baz();

            using (Fake.CreateScope())
            {
                foo.Baz();

                using (Fake.CreateScope())
                {
                    foo.Baz();
                }
            }

            Assert.That(Fake.GetCalls(foo).Count(), Is.EqualTo(3));
        }

        [Test]
        public void Rules_should_only_be_valid_within_the_current_scope()
        {
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => rule.IsApplicableTo(A<IFakeObjectCall>._)).Returns(true);

            using (Fake.CreateScope())
            {
                fake.AddRuleFirst(rule);
            }

            (fake.Object as IFoo).Bar();

            A.CallTo(() => rule.Apply(A<IInterceptedFakeObjectCall>._)).MustNotHaveHappened();
        }

        [Test]
        public void Object_properties_has_property_behavior_when_not_configured()
        {
            var foo = A.Fake<IFoo>();

            foo.SomeProperty = 10;

            Assert.That(foo.SomeProperty, Is.EqualTo(10));
        }

        [Test]
        public void Object_properties_be_set_directly_and_configured_as_methods_interchangeably()
        {
            var foo = A.Fake<IFoo>();

            A.CallTo(() => foo.SomeProperty).Returns(2);
            Assert.That(foo.SomeProperty, Is.EqualTo(2));

            foo.SomeProperty = 5;
            Assert.That(foo.SomeProperty, Is.EqualTo(5));

            A.CallTo(() => foo.SomeProperty).Returns(20);
            Assert.That(foo.SomeProperty, Is.EqualTo(20));

            foo.SomeProperty = 10;
            Assert.That(foo.SomeProperty, Is.EqualTo(10));
        }

        [Test]
        public void Properties_should_be_set_to_fake_objects_when_property_type_is_fakeable_and_the_property_is_not_explicitly_set()
        {
            var foo = A.Fake<IFoo>();

            Assert.That(foo.ChildFoo, new IsFakeConstraint());
        }

        [Test]
        public void Non_configured_property_should_have_same_fake_instance_when_accessed_twice_when_property_is_internal()
        {
            // Arrange
            var foo = A.Fake<Foo>();

            // Act

            // Assert
            Assert.That(foo.InternalVirtualFakeableProperty, Is.SameAs(foo.InternalVirtualFakeableProperty));
        }

        [Test]
        public void Property_should_return_set_value_when_property_is_internal()
        {
            // Arrange
            var foo = A.Fake<Foo>();
            var value = A.Fake<IFoo>();

            // Act
            foo.InternalVirtualFakeableProperty = value;

            // Assert
            Assert.That(foo.InternalVirtualFakeableProperty, Is.SameAs(value));
        }

        [Test]
        public void RemoveRule_should_remove_the_specified_rule_from_the_fake()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = ExpressionHelper.CreateRule<IFoo>(x => x.Bar());
            fake.AddRuleFirst(rule);

            // Act
            fake.RemoveRule(rule);

            // Assert
            Assert.That(fake.Rules, Has.None.EqualTo(rule));
        }

        [Test]
        public void RemoveRule_should_do_nothing_when_rule_does_not_exist_in_fake()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = ExpressionHelper.CreateRule<IFoo>(x => x.Bar());

            // Act
            fake.RemoveRule(rule);

            // Assert
            Assert.That(fake.Rules, Has.None.EqualTo(rule));
        }

        [Test]
        public void RemoveRule_should_be_null_guarded()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                fake.RemoveRule(ExpressionHelper.CreateRule<IFoo>(x => x.Bar())));
        }

        [Test]
        public void AddRuleLast_should_add_rule()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();

            // Act
            fake.AddRuleLast(rule);

            // Assert
            Assert.That(fake.Rules, Has.Some.SameAs(rule));
        }

        [Test]
        public void AddRuleLast_should_add_rule_last_among_the_user_specified_rules()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();

            // Act
            fake.AddRuleFirst(A.Fake<IFakeObjectCallRule>());
            fake.AddRuleLast(rule);

            // Assert
            Assert.That(fake.AllUserRules.Last.Value.Rule, Is.SameAs(rule));
        }

        [Test]
        public void Should_return_fake_and_type_when_ToString_is_intercepted_but_not_configured()
        {
            this.Should_set_default_return_value_when_object_method_has_not_been_configured(typeof(object).GetMethod("ToString"), x => "Faked FakeItEasy.Tests.IFoo");
        }

        [Test]
        public void Should_return_hash_code_of_fake_manager_when_GetHashCode_has_not_been_configured()
        {
            this.Should_set_default_return_value_when_object_method_has_not_been_configured(typeof(object).GetMethod("GetHashCode"), x => x.GetHashCode());
        }

        [Test]
        public void Should_return_false_for_equals_method_when_fake_managers_are_not_the_same()
        {
            // Arrange
            var interceptingProxy = new FakedProxyWithManagerSpecified { FakeManager = new FakeManager() };
            var proxyPassedToEquals = new FakedProxyWithManagerSpecified { FakeManager = new FakeManager() };

            var equalsMethod = typeof(object).GetMethod("Equals", new[] { typeof(object) });

            var interceptedCall = A.Fake<IWritableFakeObjectCall>();
            A.CallTo(() => interceptedCall.Method).Returns(equalsMethod);
            A.CallTo(() => interceptedCall.FakedObject).Returns(proxyPassedToEquals);
            A.CallTo(() => interceptedCall.Arguments).Returns(new ArgumentCollection(new object[] { proxyPassedToEquals }, interceptedCall.Method));

            var eventRaiser = A.Fake<ICallInterceptedEventRaiser>();

            interceptingProxy.FakeManager.AttachProxy(typeof(FakedProxyWithManagerSpecified), interceptingProxy, eventRaiser);

            // Act
            eventRaiser.CallWasIntercepted += Raise.With(new CallInterceptedEventArgs(interceptedCall));

            // Assert
            A.CallTo(() => interceptedCall.SetReturnValue(false)).MustHaveHappened();
        }

        [Test]
        public void Should_return_true_for_equals_method_when_fake_managers_are_the_same()
        {
            // Arrange
            var interceptingProxy = new FakedProxyWithManagerSpecified { FakeManager = new FakeManager() };
            var proxyPassedToEquals = new FakedProxyWithManagerSpecified { FakeManager = interceptingProxy.FakeManager };

            var equalsMethod = typeof(object).GetMethod("Equals", new[] { typeof(object) });

            var interceptedCall = A.Fake<IWritableFakeObjectCall>();
            A.CallTo(() => interceptedCall.Method).Returns(equalsMethod);
            A.CallTo(() => interceptedCall.FakedObject).Returns(proxyPassedToEquals);
            A.CallTo(() => interceptedCall.Arguments).Returns(new ArgumentCollection(new object[] { proxyPassedToEquals }, interceptedCall.Method));

            var eventRaiser = A.Fake<ICallInterceptedEventRaiser>();
            interceptingProxy.FakeManager.AttachProxy(typeof(FakedProxyWithManagerSpecified), interceptingProxy, eventRaiser);

            // Act
            eventRaiser.CallWasIntercepted += Raise.With(new CallInterceptedEventArgs(interceptedCall));

            // Assert
            A.CallTo(() => interceptedCall.SetReturnValue(true)).MustHaveHappened();
        }

        [Test]
        public void Call_should_not_be_recorded_when_DoNotRecordCall_has_been_called()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var rule = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => rule.IsApplicableTo(A<IFakeObjectCall>._)).Returns(true);
            A.CallTo(() => rule.Apply(A<IInterceptedFakeObjectCall>._)).Invokes(x => x.Arguments.Get<IInterceptedFakeObjectCall>(0).DoNotRecordCall());

            Fake.GetFakeManager(fake).AddRuleFirst(rule);

            // Act
            fake.Bar();

            // Assert
            Assert.That(Fake.GetCalls(fake), Is.Empty);
        }

        [Test]
        public void AttachProxy_should_set_proxy()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();

            var proxy = A.Dummy<IFoo>();

            // Act
            fake.AttachProxy(typeof(object), proxy, A.Dummy<ICallInterceptedEventRaiser>());

            // Assert
            Assert.That(fake.Object, Is.SameAs(proxy));
        }

        [Test]
        public void AttachProxy_should_set_the_fake_type()
        {
            // Arrange
            var fake = this.CreateFakeManager<IFoo>();

            // Act
            fake.AttachProxy(typeof(IFoo), A.Dummy<IFoo>(), A.Dummy<ICallInterceptedEventRaiser>());

            // Assert
            Assert.That(fake.FakeObjectType, Is.EqualTo(typeof(IFoo)));
        }

        [Test]
        public void AttachProxy_should_configure_manager_to_intercept_calls()
        {
            var fake = this.CreateFakeManager<IFoo>();
            ////var proxy = this.CreateProxyResult<IFoo>();
            var eventRaiser = A.Fake<ICallInterceptedEventRaiser>();

            var call = A.Fake<IWritableFakeObjectCall>();
            call.Configure().CallsTo(x => x.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            // Act
            fake.AttachProxy(typeof(object), A.Dummy<IFoo>(), eventRaiser);

            // Assert
            var rule = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => rule.IsApplicableTo(call)).Returns(true);

            fake.AddRuleFirst(rule);

            eventRaiser.CallWasIntercepted += Raise.With(new CallInterceptedEventArgs(call));

            A.CallTo(() => rule.Apply(A<IInterceptedFakeObjectCall>._)).MustHaveHappened();
        }

        [Test]
        public void Should_clear_all_added_rules_when_calling_clear_user_rules()
        {
            // Arrange
            var manager = new FakeManager();
            manager.AddRuleFirst(A.Dummy<IFakeObjectCallRule>());
            manager.AddRuleLast(A.Dummy<IFakeObjectCallRule>());

            // Act
            manager.ClearUserRules();

            // Assert
            Assert.That(manager.AllUserRules, Is.Empty);
        }

        [Test]
        public void Should_invoke_listener_when_call_is_intercepted()
        {
            // Arrange
            var interceptedCall = A.Dummy<IWritableFakeObjectCall>();

            var listener = A.Fake<IInterceptionListener>();
            var manager = new RaisableFakeManager();

            manager.AddInterceptionListener(listener);

            // Act
            manager.RaiseCallIntercepted(new CallInterceptedEventArgs(interceptedCall));

            // Assert
            A.CallTo(() => listener.OnBeforeCallIntercepted(interceptedCall)).MustHaveHappened();
        }

        [Test]
        public void Should_invoke_listener_after_call_has_been_intercepted()
        {
            // Arrange
            var interceptedCall = A.Fake<IWritableFakeObjectCall>(x => x.Implements(typeof(ICompletedFakeObjectCall)));
            A.CallTo(() => interceptedCall.AsReadOnly()).Returns((ICompletedFakeObjectCall)interceptedCall);

            var listener = A.Fake<IInterceptionListener>();
            var manager = new RaisableFakeManager();

            var selectedRule = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => selectedRule.IsApplicableTo(interceptedCall)).Returns(true);

            manager.AddRuleFirst(selectedRule);
            manager.AddInterceptionListener(listener);

            // Act
            manager.RaiseCallIntercepted(new CallInterceptedEventArgs(interceptedCall));

            // Assert
            A.CallTo(() => listener.OnAfterCallIntercepted((ICompletedFakeObjectCall)interceptedCall, selectedRule)).MustHaveHappened();
        }

        [Test]
        public void Should_invoke_listener_after_call_has_been_intercepted_when_application_of_rule_throws()
        {
            // Arrange
            var interceptedCall = A.Fake<IWritableFakeObjectCall>(x => x.Implements(typeof(ICompletedFakeObjectCall)));
            A.CallTo(() => interceptedCall.AsReadOnly()).Returns((ICompletedFakeObjectCall)interceptedCall);

            var listener = A.Fake<IInterceptionListener>();
            var manager = new RaisableFakeManager();

            var selectedRule = A.Fake<IFakeObjectCallRule>();
            A.CallTo(() => selectedRule.IsApplicableTo(interceptedCall)).Returns(true);
            A.CallTo(() => selectedRule.Apply(A<IInterceptedFakeObjectCall>._)).Throws(new InvalidOperationException());

            manager.AddRuleFirst(selectedRule);
            manager.AddInterceptionListener(listener);

            // Act
            Record.Exception(() => manager.RaiseCallIntercepted(new CallInterceptedEventArgs(interceptedCall)));

            // Assert
            A.CallTo(() => listener.OnAfterCallIntercepted((ICompletedFakeObjectCall)interceptedCall, selectedRule)).MustHaveHappened();
        }

        [Test]
        public void Should_invoke_listeners_in_the_correct_order()
        {
            // Arrange
            var manager = new RaisableFakeManager();
            var listener1 = A.Fake<IInterceptionListener>();
            var listener2 = A.Fake<IInterceptionListener>();

            // Act
            manager.AddInterceptionListener(listener1);
            manager.AddInterceptionListener(listener2);

            // Assert
            using (var scope = Fake.CreateScope())
            {
                manager.RaiseCallIntercepted(new CallInterceptedEventArgs(A.Dummy<IWritableFakeObjectCall>()));

                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => listener2.OnBeforeCallIntercepted(A<IFakeObjectCall>._)).MustHaveHappened();
                    A.CallTo(() => listener1.OnBeforeCallIntercepted(A<IFakeObjectCall>._)).MustHaveHappened();
                    A.CallTo(() => listener1.OnAfterCallIntercepted(A<ICompletedFakeObjectCall>._, A<IFakeObjectCallRule>._)).MustHaveHappened();
                    A.CallTo(() => listener2.OnAfterCallIntercepted(A<ICompletedFakeObjectCall>._, A<IFakeObjectCallRule>._)).MustHaveHappened();
                }
            }
        }

        private static FakeCallRule CreateApplicableInterception()
        {
            return new FakeCallRule { IsApplicableTo = x => true };
        }

        private FakeManager CreateFakeManager<T>()
        {
            var result = A.Fake<T>();
            this.MakeSureThatWeakReferenceDoesNotGetCollected(result);
            return Fake.GetFakeManager(result);
        }

        private void Should_set_default_return_value_when_object_method_has_not_been_configured(MethodInfo interceptedMethod, Func<FakeManager, object> expectedValue)
        {
            // Arrange
            var manager = new FakeManager();

            var interceptedCall = A.Fake<IWritableFakeObjectCall>();
            A.CallTo(() => interceptedCall.Method).Returns(interceptedMethod);

            var eventRaiser = A.Fake<ICallInterceptedEventRaiser>();
            manager.AttachProxy(typeof(IFoo), A.Fake<IFoo>(), eventRaiser);

            // Act
            eventRaiser.CallWasIntercepted += Raise.With(new CallInterceptedEventArgs(interceptedCall));

            // Assert
            A.CallTo(() => interceptedCall.SetReturnValue(expectedValue.Invoke(manager))).MustHaveHappened();
        }

        private void MakeSureThatWeakReferenceDoesNotGetCollected<T>(T result)
        {
            this.createdFakes.Add(result);
        }

        public class TypeWithNoDefaultConstructorButAllArgumentsFakeable
        {
            public TypeWithNoDefaultConstructorButAllArgumentsFakeable(IFoo foo, IFormatProvider formatProvider)
            {
                this.Foo = foo;
                this.FormatProvider = formatProvider;
            }

            public IFoo Foo { get; set; }

            public IFormatProvider FormatProvider { get; set; }
        }

        private class FakedProxyWithManagerSpecified
            : ITaggable
        {
            public FakeManager FakeManager
            {
                get { return (FakeManager)this.Tag; }
                set { this.Tag = value; }
            }

            public object Tag
            {
                get;
                set;
            }
        }

        private sealed class RaisableFakeManager
            : FakeManager
        {
            private ICallInterceptedEventRaiser raiser;

            public RaisableFakeManager()
            {
                this.raiser = A.Fake<ICallInterceptedEventRaiser>();
                this.AttachProxy(typeof(object), new object(), this.raiser);
            }

            public void RaiseCallIntercepted(CallInterceptedEventArgs eventArgs)
            {
                this.raiser.CallWasIntercepted += Raise.With(eventArgs);
            }
        }
    }
}
