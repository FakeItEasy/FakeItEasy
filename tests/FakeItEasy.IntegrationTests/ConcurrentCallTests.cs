namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ConcurrentCallTests
    {
        public static IEnumerable<object?[]> UserRuleMutatingActions =>
            TestCases.FromObject(
                new GetUnconfiguredProperty(),
                new SetUnconfiguredProperty(),
                new AddRuleFirst(),
                new AddRuleLast(),
                new RemoveRule(),
                new ClearConfiguration(),
                new AddConfigurationViaACallTo(),
                new AddConfigurationViaThen());

        [Theory]
        [MemberData(nameof(UserRuleMutatingActions))]
        public void User_rule_mutating_action_concurrent_with_member_invocation_should_not_throw_concurrent_modification_exception(RuleMutatingAction mutatingAction)
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            // Some mutating actions require a little setup before we start iterating over the user rules
            mutatingAction.Setup(fake);

            // Use locks to ensure that the first member access call is still looping over configured rules
            // when the user rule list-mutating action occurs:
            // slow member invocation apply    |-----------------------------|
            // fast rule list-mutating action         |---------------|
            var slowMemberInvocationApplyCheckStartedLock = new ManualResetEventSlim(initialState: false);
            var fastRuleMutatingActionFinishedLock = new ManualResetEventSlim(initialState: false);

            A.CallTo(() => fake.IntProperty).WhenArgumentsMatch(call =>
            {
                slowMemberInvocationApplyCheckStartedLock.Set();

                // Timing out isn't ideal, as it introduces the possibility that we'll just finish
                // the test before the fact rule list mutating-action is complete, but it's unlikely,
                // and we can't wait forever, or any lock on the user rules will deadlock the test.
                // When written (before serializing access to user rules) the tests did fail
                // consistently with a timeout of 0.1 seconds.
                fastRuleMutatingActionFinishedLock.Wait(TimeSpan.FromSeconds(0.1));

                // Return false so we keep iterating through the list of user rules. Otherwise, we
                // never suffer the "collection was modified during iteration" failure case.
                return false;
            }).Returns(0);

            // Act
            var longRunningTask = Task.Run(() => fake.IntProperty);

            var shortRunningTask = Task.Run(() =>
            {
                slowMemberInvocationApplyCheckStartedLock.Wait();
                mutatingAction.MutateRules(fake);
                fastRuleMutatingActionFinishedLock.Set();
            });

            var exception = Record.Exception(() => Task.WaitAll(longRunningTask, shortRunningTask));

            // Assert
            exception.Should().BeNull();
        }

        public abstract class RuleMutatingAction
        {
            public virtual void Setup(IFoo fake)
            {
                // most rules won't need a setup, which is run before iterating over the user rules
            }

            public abstract void MutateRules(IFoo fake);
        }

        private class GetUnconfiguredProperty : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake)
            {
                var ignored = fake.StringProperty;
            }
        }

        private class SetUnconfiguredProperty : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake) => fake.StringProperty = "a string";
        }

        private class AddRuleFirst : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake) => Fake.GetFakeManager(fake).AddRuleFirst(A.Dummy<IFakeObjectCallRule>());
        }

        private class AddRuleLast : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake) => Fake.GetFakeManager(fake).AddRuleLast(A.Dummy<IFakeObjectCallRule>());
        }

        private class RemoveRule : RuleMutatingAction
        {
            private IFakeObjectCallRule? ruleToRemove;

            public override void Setup(IFoo fake)
            {
                // Make sure we have enough rules to remove. Also that we know a rule that can
                // be removed.
                A.CallTo(() => fake.IntProperty).Returns(1);
                A.CallTo(() => fake.IntProperty).Returns(1);
                this.ruleToRemove = Fake.GetFakeManager(fake).Rules.Last();
            }

            public override void MutateRules(IFoo fake)
            {
                Fake.GetFakeManager(fake).RemoveRule(this.ruleToRemove!);
            }
        }

        private class ClearConfiguration : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake) => Fake.ClearConfiguration(fake);
        }

        private class AddConfigurationViaACallTo : RuleMutatingAction
        {
            public override void MutateRules(IFoo fake) => A.CallTo(() => fake.IntProperty).Returns(1);
        }

        private class AddConfigurationViaThen : RuleMutatingAction
        {
            // If the mutating action performs
            // A.CallTo(() => fake.IntProperty).Returns(1).Once().Then.Returns(2)
            // all at once, there's no way to tell which Returns is interfering with the
            // traversing of the user rule list, so configure the first Returns in the Setup.
            private IThenConfiguration<IReturnValueConfiguration<int>>? firstCallConfiguration;

            public override void Setup(IFoo fake)
            {
                this.firstCallConfiguration = A.CallTo(() => fake.IntProperty).Returns(1).Once();
            }

            public override void MutateRules(IFoo fake)
            {
                this.firstCallConfiguration!.Then.Returns(2);
            }
        }
    }
}
