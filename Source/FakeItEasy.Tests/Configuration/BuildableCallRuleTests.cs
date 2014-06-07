namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class BuildableCallRuleTests
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] defaultReturnValueCases = TestCases.Create(
            new
            {
                MethodName = "IntReturn",
                ExpectedReturnValue = (object)0
            },
            new
            {
                MethodName = "StringReturn",
                ExpectedReturnValue = (object)null
            },
            new
            {
                MethodName = "SelfReturn",
                ExpectedReturnValue = (object)null
            }).AsTestCaseSource();

        private TestableCallRule rule;

        private interface IOutAndRef
        {
            void OutAndRef(object input, out int first, string input2, ref string second, string input3);
        }

        private interface IHaveDifferentReturnValues
        {
            int IntReturn();

            string StringReturn();

            IHaveDifferentReturnValues SelfReturn();
        }

        [SetUp]
        public void Setup()
        {
            this.rule = new TestableCallRule();
        }

        [Test]
        public void Apply_should_invoke_all_actions_in_the_actions_collection()
        {
            // Arrange
            bool firstWasCalled = false;
            bool secondWasCalled = false;
            this.rule.Actions.Add(x => firstWasCalled = true);
            this.rule.Actions.Add(x => secondWasCalled = true);

            this.rule.Applicator = x => { };

            // Act
            this.rule.Apply(A.Fake<IInterceptedFakeObjectCall>());

            // Assert
            firstWasCalled.Should().BeTrue();
            secondWasCalled.Should().BeTrue();
        }

        [Test]
        public void Apply_should_pass_the_call_to_specified_actions()
        {
            var call = A.Fake<IInterceptedFakeObjectCall>();
            IFakeObjectCall passedCall = null;

            this.rule.Applicator = x => { };
            this.rule.Actions.Add(x => passedCall = x);

            this.rule.Apply(call);

            passedCall.Should().BeSameAs(call);
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
            this.rule.OutAndRefParametersValueProducer = (x) => { return new object[] { 1, "foo" }; };
            this.rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));

            // Act
            this.rule.Apply(call);

            A.CallTo(() => call.SetArgumentValue(1, 1)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(3, "foo")).MustHaveHappened();
        }

        [Test]
        public void Apply_should_throw_when_OutAndRefParametersValues_length_differs_from_the_number_of_out_and_ref_parameters_in_the_call()
        {
            // Arrange
            this.rule.OutAndRefParametersValueProducer = (x) => { return new object[] { 1, "foo", "bar" }; };
            this.rule.Applicator = x => { };

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod("OutAndRef"));

            var exception = Record.Exception(() =>
                this.rule.Apply(call));

            exception.Should().BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.");
        }

        [TestCaseSource("defaultReturnValueCases")]
        public void Apply_should_set_return_value_to_default_value_when_applicator_is_not_set(string methodName, object expectedResponse)
        {
            // Arrange
            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IHaveDifferentReturnValues).GetMethod(methodName));

            // Act
            this.rule.Apply(call);

            // Assert
            A.CallTo(() => call.SetReturnValue(expectedResponse))
                .MustHaveHappened();
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool Apply_should_return_return_value_from_on_is_applicable_to_when_a_where_predicate_is_not_set(bool resultFromOnIsApplicableTo)
        {
            // Arrange
            this.rule.ReturnValueFromOnIsApplicableTo = resultFromOnIsApplicableTo;

            // Act
            var result = this.rule.IsApplicableTo(null);

            // Assert
            return result;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "andon", Justification = "False positive")]
        [Test]
        public void IsApplicableTo_should_return_false_when_a_predicate_fails_and_on_is_applicable_to_passes()
        {
            // Arrange
            this.rule.ReturnValueFromOnIsApplicableTo = true;

            this.rule.ApplyWherePredicate(x => true, x => { });
            this.rule.ApplyWherePredicate(x => false, x => { });

            // Act
            var isThisRuleApplicable = this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>());

            // Assert
            isThisRuleApplicable.Should().BeFalse();
        }

        [Test]
        public void Should_not_call_OnIsApplicableTo_when_a_where_predicate_returns_false()
        {
            // Arrange
            this.rule.ApplyWherePredicate(x => false, x => { });

            // Act
            this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>());

            // Assert
            this.rule.OnIsApplicableToWasCalled.Should().BeFalse();
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
            var isApplicableTo = this.rule.IsApplicableTo(call);

            // Assert
            isApplicableTo.Should().BeTrue();
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
            writer.Builder.ToString().Should().Be("description");
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
            var expectedDescription =
@"description
  where description of first where
  and description of second where";

            descriptionWriter.Builder.ToString().Should().Be(expectedDescription);
        }

        private class TestableCallRule
            : BuildableCallRule
        {
            public TestableCallRule()
            {
                this.ReturnValueFromOnIsApplicableTo = true;
                this.DescriptionOfValidCallReturnValue = string.Empty;
            }

            public bool ReturnValueFromOnIsApplicableTo { get; set; }

            public string DescriptionOfValidCallReturnValue { get; set; }

            public bool OnIsApplicableToWasCalled { get; set; }

            public override string DescriptionOfValidCall
            {
                get { return this.DescriptionOfValidCallReturnValue; }
            }

            public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
            {
            }

            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                this.OnIsApplicableToWasCalled = true;
                return this.ReturnValueFromOnIsApplicableTo;
            }
        }
    }
}
