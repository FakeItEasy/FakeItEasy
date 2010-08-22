using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;
using FakeItEasy.Core;
using System.Collections.Generic;

namespace FakeItEasy.Tests.Configuration
{

    [TestFixture]
    public class BuildableCallRuleTests
    {
        private BuildableCallRule CreateRule()
        {
            return new TestableCallRule();
        }

        [Test]
        public void Apply_should_invoke_all_actions_in_the_actions_collection()
        {
            // Arrange
            var rule = this.CreateRule();

            bool firstWasCalled = false;
            bool secondWasCalled = false;
            rule.Actions.Add(x => firstWasCalled = true);
            rule.Actions.Add(x => secondWasCalled = true);

            rule.Applicator = x => { };

            // Act
            rule.Apply(A.Fake<IInterceptedFakeObjectCall>());
            
            // Assert
            Assert.That(firstWasCalled, Is.True);
            Assert.That(secondWasCalled, Is.True);
        }

        [Test]
        public void Apply_should_pass_the_call_to_specified_actions()
        {
            var rule = this.CreateRule();

            var call = A.Fake<IInterceptedFakeObjectCall>();
            IFakeObjectCall passedCall = null;

            rule.Applicator = x => { };
            rule.Actions.Add(x => passedCall = x);

            rule.Apply(call);

            Assert.That(passedCall, Is.SameAs(call));
        }

        [Test]
        public void Apply_should_call_CallBaseMethod_on_intercepted_call_when_CallBaseMethod_is_set_to_true()
        {
            var rule = this.CreateRule();

            var call = A.Fake<IInterceptedFakeObjectCall>();

            rule.Applicator = x => { };
            rule.CallBaseMethod = true;
            rule.Apply(call);

            A.CallTo(() => call.CallBaseMethod()).MustHaveHappened();
        }


        [Test]
        public void Apply_should_set_ref_and_out_parameters_when_specified()
        {
            // Arrange
            var rule = this.CreateRule();

            rule.OutAndRefParametersValues = new object[] { 1, "foo" };
            rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();
            
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));

            // Act
            rule.Apply(call);

            A.CallTo(() => call.SetArgumentValue(1, 1)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(3, "foo")).MustHaveHappened();
        }

        [Test]
        public void Apply_should_throw_when_OutAndRefParametersValues_length_differes_from_the_number_of_out_and_ref_parameters_in_the_call()
        {
            var rule = this.CreateRule();

            rule.OutAndRefParametersValues = new object[] { 1, "foo", "bar" };
            rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));
            
            var ex = Assert.Throws<InvalidOperationException>(() =>
                rule.Apply(call));
            Assert.That(ex.Message, Is.EqualTo("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call."));
        }

        private interface IOutAndRef
        {
            void OutAndRef(object input, out int first, string input2, ref string second, string input3);
        }

        private class TestableCallRule
            : BuildableCallRule
        {
            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return true;
            }

            public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                
            }

            public override string DescriptionOfValidCall
            {
                get 
                {
                    return string.Empty;
                }
            }
        }

    }
}
