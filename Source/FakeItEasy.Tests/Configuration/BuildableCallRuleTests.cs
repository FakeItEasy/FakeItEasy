using System;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using NUnit.Framework;

namespace FakeItEasy.Tests.Configuration
{

    [TestFixture]
    public class BuildableCallRuleTests
    {
        private TestableCallRule rule;

        [SetUp]
        public void SetUp()
        {
            this.rule = this.CreateRule();
        }

        private TestableCallRule CreateRule()
        {
            return new TestableCallRule();
        }

        [Test]
        public void Apply_should_invoke_all_actions_in_the_actions_collection()
        {
            // Arrange
            bool firstWasCalled = false;
            bool secondWasCalled = false;
            rule.Actions.Add(x => firstWasCalled = true);
            rule.Actions.Add(x => secondWasCalled = true);

            this.rule.Applicator = x => { };

            // Act
            this.rule.Apply(A.Fake<IInterceptedFakeObjectCall>());
            
            // Assert
            Assert.That(firstWasCalled, Is.True);
            Assert.That(secondWasCalled, Is.True);
        }

        [Test]
        public void Apply_should_pass_the_call_to_specified_actions()
        {
            

            var call = A.Fake<IInterceptedFakeObjectCall>();
            IFakeObjectCall passedCall = null;

            this.rule.Applicator = x => { };
            this.rule.Actions.Add(x => passedCall = x);

            this.rule.Apply(call);

            Assert.That(passedCall, Is.SameAs(call));
        }

        [Test]
        public void Apply_should_call_CallBaseMethod_on_intercepted_call_when_CallBaseMethod_is_set_to_true()
        {
            

            var call = A.Fake<IInterceptedFakeObjectCall>();

            this.rule.Applicator = x => { };
            this.rule.CallBaseMethod = true;
            this.rule.Apply(call);

            A.CallTo(() => call.CallBaseMethod()).MustHaveHappened();
        }


        [Test]
        public void Apply_should_set_ref_and_out_parameters_when_specified()
        {
            // Arrange
            

            this.rule.OutAndRefParametersValues = new object[] { 1, "foo" };
            this.rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();
            
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));

            // Act
            this.rule.Apply(call);

            A.CallTo(() => call.SetArgumentValue(1, 1)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(3, "foo")).MustHaveHappened();
        }

        [Test]
        public void Apply_should_throw_when_OutAndRefParametersValues_length_differes_from_the_number_of_out_and_ref_parameters_in_the_call()
        {
            

            this.rule.OutAndRefParametersValues = new object[] { 1, "foo", "bar" };
            this.rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));
            
            var ex = Assert.Throws<InvalidOperationException>(() =>
                this.rule.Apply(call));
            Assert.That(ex.Message, Is.EqualTo("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call."));
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool Apply_should_return_return_value_from_on_is_applicable_to_when_no_where_predicate_is_set(bool resultFromOnIsApplicableTo)
        {
            // Arrange
            
            this.rule.ReturnValueFromOnIsApplicableTo = resultFromOnIsApplicableTo;

            // Act

            // Assert
            return this.rule.IsApplicableTo(null);
        }

        [Test]
        public void Apply_should_return_false_when_a_predicate_fails_and_on_is_applicable_to_passes()
        {
            // Arrange
            
            this.rule.ReturnValueFromOnIsApplicableTo = true;

            this.rule.ApplyWherePredicate(x => true, x => { });
            this.rule.ApplyWherePredicate(x => false, x => { });

            // Act

            // Assert
            Assert.That(this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>()), Is.False);
        }

        [Test]
        public void Apply_should_return_true_when_on_is_applicable_to_is_true_and_all_where_predicates_returns_true()
        {
            // Arrange
            
            this.rule.ReturnValueFromOnIsApplicableTo = true;

            this.rule.ApplyWherePredicate(x => true, x => { });
            this.rule.ApplyWherePredicate(x => true, x => { });

            // Act

            // Assert
            Assert.That(this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>()), Is.True);
        }

        [Test]
        public void Should_pass_call_to_where_predicates()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            A.CallTo(() => call.ToString()).Returns("foo");

            
            this.rule.ApplyWherePredicate(x => x.ToString() == "foo", x => { });

            // Act
            
            // Assert
            Assert.That(this.rule.IsApplicableTo(call), Is.True);
        }

        [Test]
        public void Should_write_description_of_valid_call_by_calling_the_description_property()
        {
            // Arrange
            
            this.rule.DescriptionOfValidCallReturnValue = "description";

            var writer = new StringBuilderOutputWriter();

            // Act
            this.rule.WriteDescriptionOfValidCall(writer);

            // Assert
            Assert.That(writer.Builder.ToString(), Is.EqualTo(@"description
"));
        }

        [Test]
        public void Should_append_where_predicates_to_description_correctly()
        {
            // Arrange
            this.rule.DescriptionOfValidCallReturnValue = "description";
            this.rule.ApplyWherePredicate(x => true, x => x.Write("description of first where"));
            this.rule.ApplyWherePredicate(x => true, x => x.Write("description of second where"));

            var descriptionWriter = new StringBuilderOutputWriter();
            
            // Act
            this.rule.WriteDescriptionOfValidCall(descriptionWriter);

            // Assert
            Assert.That(descriptionWriter.Builder.ToString(),
                Is.EqualTo(@"description
    where description of first where
    and description of second where
"));
        }

        [Test]
        public void Should_be_able_to_specify_expressions_where_predicates()
        {
            // Arrange
            this.rule.DescriptionOfValidCallReturnValue = "description";
            this.rule.ApplyWherePredicate(x => x.Arguments.Count > 0);
            
            var descriptionWriter = new StringBuilderOutputWriter();

            // Act
            this.rule.WriteDescriptionOfValidCall(descriptionWriter);

            // Assert
            Assert.That(descriptionWriter.Builder.ToString(),
                Is.EqualTo(@"description
    where x => (x.Arguments.Count > 0)
"));
        }

        private interface IOutAndRef
        {
            void OutAndRef(object input, out int first, string input2, ref string second, string input3);
        }

        private class TestableCallRule
            : BuildableCallRule
        {
            public bool ReturnValueFromOnIsApplicableTo = true;
            public string DescriptionOfValidCallReturnValue = string.Empty;

            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.ReturnValueFromOnIsApplicableTo;
            }

            public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                
            }

            public override string DescriptionOfValidCall
            {
                get { return this.DescriptionOfValidCallReturnValue; }
            }
        }
    }
}
