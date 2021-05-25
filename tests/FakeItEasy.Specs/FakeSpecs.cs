namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class FakeSpecs
    {
        public interface IFoo
        {
            void AMethod();

            string AnotherMethod();

            void AnotherMethod(string text);

            object AMethodReturningAnObject();
        }

        public interface IWillKeepMyImplicitConfiguration
        {
            string AMethod();
        }

        [Scenario]
        public static void NonGenericCallsSuccess(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod("houseboat");
                });

            "When I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "Then the calls made to the Fake will be returned"
                .x(() =>
                    completedCalls.Select(call => call.Method.Name)
                        .Should()
                        .Equal("AMethod", "AnotherMethod", "AnotherMethod"));
        }

        [Scenario]
        public static void MatchingCallsWithNoMatches(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls,
            IEnumerable<ICompletedFakeObjectCall> matchedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod("houseboat");
                });

            "And I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "When I use Matching to find calls to a method with no matches"
                .x(() => matchedCalls = completedCalls.Matching<IFoo>(c => c.AnotherMethod("hovercraft")));

            "Then it finds no calls"
                .x(() => matchedCalls.Should().BeEmpty());
        }

        [Scenario]
        public static void MatchingCallsWithMatches(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls,
            IEnumerable<ICompletedFakeObjectCall> matchedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod("houseboat");
                });

            "And I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "When I use Matching to find calls to a method with a match"
                .x(() => matchedCalls = completedCalls.Matching<IFoo>(c => c.AnotherMethod("houseboat")));

            "Then it finds the matching call"
                .x(() => matchedCalls.Select(c => c.Method.Name).Should().Equal("AnotherMethod"));
        }

        [Scenario]
        public static void ClearRecordedCalls(IFoo fake)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod("houseboat");
                });

            "When I clear the recorded calls"
                .x(() => Fake.ClearRecordedCalls(fake));

            "Then the recorded call list is empty"
                .x(() => Fake.GetCalls(fake).Should().BeEmpty());
        }

        [Scenario]
        public static void ClearConfiguration(IFoo fake, bool configuredBehaviorWasUsed)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => fake.AMethod()).Invokes(() => configuredBehaviorWasUsed = true));

            "When I clear the configuration"
#pragma warning disable CS0618 // ClearConfiguration is obsolete
                .x(() => Fake.ClearConfiguration(fake));
#pragma warning restore CS0618 // ClearConfiguration is obsolete

            "And I execute the previously-configured method"
                .x(() => fake.AMethod());

            "Then previously-configured behavior is not executed"
                .x(() => configuredBehaviorWasUsed.Should().BeFalse());
        }

        [Scenario]
        public static void ResetToInitialConfigurationResetsLateConfiguration(IFoo fake, string configuredResult, string resetResult)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => fake.AnotherMethod()).Returns("configured value"));

            "And I call the method"
                .x(() => configuredResult = fake.AnotherMethod());

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I call the previously-configured method"
                .x(() => resetResult = fake.AnotherMethod());

            "Then the first result is the configured value"
                .x(() => configuredResult.Should().Be("configured value"));

            "And the second result is the default value"
                .x(() => resetResult.Should().Be(string.Empty));
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsStrictness(IFoo fake, string configuredResult, Exception exception)
        {
            "Given a strict Fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => fake.AnotherMethod()).Returns("avoiding strictness"));

            "And I call the method"
                .x(() => configuredResult = fake.AnotherMethod());

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I execute a method"
                .x(() => exception = Record.Exception(() => fake.AMethod()));

            "Then the first call returns the configured value"
                .x(() => configuredResult.Should().Be("avoiding strictness"));

            "And the second call is rejected, since the Fake is still strict"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsWrappingBehavior(
            WrappingFakeSpecs.Foo realObject,
            WrappingFakeSpecs.IFoo wrapper,
            int configuredResult,
            int wrappedResult)
        {
            "Given a real object"
                .x(() => realObject = new WrappingFakeSpecs.Foo());

            "And a Fake wrapping this object"
                .x(() => wrapper = A.Fake<WrappingFakeSpecs.IFoo>(o => o.Wrapping(realObject)));

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => wrapper.NonVoidMethod("hello")).Returns(97));

            "And I call the method"
                .x(() => configuredResult = wrapper.NonVoidMethod("hello"));

            "When I reset the wrapper's configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(wrapper));

            "And I call the previously-configured method on the wrapper"
                .x(() => wrappedResult = wrapper.NonVoidMethod("hello"));

            "Then the first call returns the configured value"
                .x(() => wrappedResult.Should().Be(5));

            "And the second call returns the value from the wrapped object"
                .x(() => wrappedResult.Should().Be(5));
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsBaseCallingBehavior(AbstractBaseClass fake, string configuredResult, string baseResult)
        {
            "Given a Fake that calls its base methods"
                .x(() => fake = A.Fake<AbstractBaseClass>(o => o.CallsBaseMethods()));

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => fake.ConcreteMethod()).Returns("don't be basic"));

            "And I call the method"
                .x(() => configuredResult = fake.ConcreteMethod());

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I call the previously-configured method on the Fake"
                .x(() => baseResult = fake.ConcreteMethod());

            "Then the first call returns the configured value"
                .x(() => configuredResult.Should().Be("don't be basic"));

            "And the second call returns the value from the base"
                .x(() => baseResult.Should().Be("result from base method"));
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsExplicitCreationOptionsConfiguration(IFoo fake, string configuredResult, string resetResult)
        {
            "Given a Fake that explicitly configures a method during creation"
                .x(() => fake = A.Fake<IFoo>(options => options.ConfigureFake(
                    f => A.CallTo(() => f.AnotherMethod()).Returns("explicitly configured value"))));

            "And I configure the method to return something else"
                .x(() => A.CallTo(() => fake.AnotherMethod()).Returns("a different value"));

            "And I call the method"
                .x(() => configuredResult = fake.AnotherMethod());

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I execute the previously-configured method"
                .x(() => resetResult = fake.AnotherMethod());

            "Then the first result is the configured value"
                .x(() => configuredResult.Should().Be("a different value"));

            "And the second result is the default value"
                .x(() => resetResult.Should().Be("explicitly configured value"));
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsImplicitCreationOptionsConfiguration(IWillKeepMyImplicitConfiguration fake, string configuredResult, string resetResult)
        {
            "Given a Fake whose options builder implicitly configures a method"
                .x(() => fake = A.Fake<IWillKeepMyImplicitConfiguration>());

            "And I configure the method to return something else"
                .x(() => A.CallTo(() => fake.AMethod()).Returns("a different value"));

            "And I call the method"
                .x(() => configuredResult = fake.AMethod());

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I execute the previously-configured method"
                .x(() => resetResult = fake.AMethod());

            "Then the first result is the configured value"
                .x(() => configuredResult.Should().Be("a different value"));

            "And the second result is the default value"
                .x(() => resetResult.Should().Be("implicitly configured value"));
        }

        [Scenario]
        public static void ResetToInitialConfigurationKeepsAutoPropertyUsedInConstructor(SetsVirtualPropertyInConstructor fake, string configuredResult, string resetResult)
        {
            "Given a Fake whose constructor sets the value of a read/write property"
                .x(() => fake = A.Fake<SetsVirtualPropertyInConstructor>());

            "And I set the value myself"
                .x(() => fake.AutoPropertySetDuringConstructorCall = "a value of my own devising");

            "And I fetch the value of the read/write property"
                .x(() => configuredResult = fake.AutoPropertySetDuringConstructorCall);

            "When I reset the configuration to its initial state"
                .x(() => Fake.ResetToInitialConfiguration(fake));

            "And I fetch the value of the read/write property"
                .x(() => resetResult = fake.AutoPropertySetDuringConstructorCall);

            "Then the first result is the value I set"
                .x(() => configuredResult.Should().Be("a value of my own devising"));

            "And the second result is the value set during the constructor call"
                .x(() => resetResult.Should().Be("value set during constructor call"));
        }

        [Scenario]
        public static void RecordedCallHasReturnValue(
            IFoo fake,
            object returnValue,
            ICompletedFakeObjectCall recordedCall)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When I call a method on the fake"
                .x(() => returnValue = fake.AMethodReturningAnObject());

            "And I retrieve the call from the recorded calls"
                .x(() => recordedCall = Fake.GetCalls(fake).Single());

            "Then the recorded call's return value should be the value that was actually returned"
                .x(() => recordedCall.ReturnValue.Should().Be(returnValue));
        }

        [Scenario]
        public static void RecordedDelegateCallHasReturnValue(
            Func<object> fake,
            object returnValue,
            ICompletedFakeObjectCall recordedCall)
        {
            "Given a fake delegate"
                .x(() => fake = A.Fake<Func<object>>());

            "When I invoke the fake delegate"
                .x(() => returnValue = fake());

            "And I retrieve the call from the recorded calls"
                .x(() => recordedCall = Fake.GetCalls(fake).Single());

            "Then the recorded call's return value should be the value that was actually returned"
                .x(() => recordedCall.ReturnValue.Should().Be(returnValue));
        }

        [Scenario]
        public static void TryGetFakeManagerWhenFakeObject(object fake, bool result, FakeManager? manager)
        {
            "Given a fake object"
                .x(() => fake = A.Fake<object>());

            "When I try to get the FakeManager for that object"
                .x(() => result = Fake.TryGetFakeManager(fake, out manager));

            "Then the result should be true"
                .x(() => result.Should().BeTrue());

            "And manager should be set"
                .x(() => manager.Should().NotBeNull());
        }

        [Scenario]
        public static void TryGetFakeManagerWhenNonFakeObject(object notFake, bool result, FakeManager? manager)
        {
            "Given a non-fake object"
                .x(() => notFake = new object());

            "When I try to get the FakeManager for that object"
                .x(() => result = Fake.TryGetFakeManager(notFake, out manager));

            "Then the result should be false"
                .x(() => result.Should().BeFalse());

            "And manager should be null"
                .x(() => manager.Should().BeNull());
        }

        [Scenario]
        public static void FakeIsFake(object fake, bool result)
        {
            "Given a fake object"
                .x(() => fake = A.Fake<object>());

            "When I check if that object is fake"
                .x(() => result = Fake.IsFake(fake));

            "Then the result should be true"
                .x(() => result.Should().BeTrue());
        }

        [Scenario]
        public static void NonFakeIsIsNotFake(object fake, bool result)
        {
            "Given a non-fake object"
                .x(() => fake = new object());

            "When I check if that object is fake"
                .x(() => result = Fake.IsFake(fake));

            "Then the result should be false"
                .x(() => result.Should().BeFalse());
        }

        public class IWillKeepMyImplicitConfigurationOptionsBuilder : FakeOptionsBuilder<IWillKeepMyImplicitConfiguration>
        {
            protected override void BuildOptions(IFakeOptions<IWillKeepMyImplicitConfiguration> options)
            {
                if (options is not null)
                {
                    options.ConfigureFake(fake => A.CallTo(() => fake.AMethod()).Returns("implicitly configured value"));
                }
            }
        }

        public class SetsVirtualPropertyInConstructor
        {
            public SetsVirtualPropertyInConstructor() =>
                this.AutoPropertySetDuringConstructorCall = "value set during constructor call";

            public virtual string AutoPropertySetDuringConstructorCall { get; set; }
        }
    }
}
