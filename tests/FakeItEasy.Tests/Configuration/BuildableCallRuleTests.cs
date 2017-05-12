namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class BuildableCallRuleTests
    {
        private readonly TestableCallRule rule;

        public BuildableCallRuleTests()
        {
            this.rule = new TestableCallRule();
        }

        private interface IOutAndRef
        {
            void OutAndRef(object input, out int first, string input2, ref string second, string input3);
        }

        private interface IHaveDifferentReturnValues
        {
            int IntReturn();

            string StringReturn();

            DummyableClass DummyableReturn();
        }

        public static IEnumerable<object> DefaultReturnValueCases()
        {
            return TestCases.FromProperties(
                new
                {
                    MethodName = nameof(IHaveDifferentReturnValues.IntReturn),
                    ExpectedReturnValue = (object)0
                },
                new
                {
                    MethodName = nameof(IHaveDifferentReturnValues.StringReturn),
                    ExpectedReturnValue = (object)string.Empty
                },
                new
                {
                    MethodName = nameof(IHaveDifferentReturnValues.DummyableReturn),
                    ExpectedReturnValue = (object)A.Dummy<DummyableClass>()
                });
        }

        [Fact]
        public void Apply_should_invoke_all_actions_in_the_actions_collection()
        {
            // Arrange
            bool firstWasCalled = false;
            bool secondWasCalled = false;
            this.rule.Actions.Add(x => firstWasCalled = true);
            this.rule.Actions.Add(x => secondWasCalled = true);

            this.rule.UseApplicator(x => { });

            // Act
            this.rule.Apply(A.Fake<IInterceptedFakeObjectCall>());

            // Assert
            firstWasCalled.Should().BeTrue();
            secondWasCalled.Should().BeTrue();
        }

        [Fact]
        public void Apply_should_pass_the_call_to_specified_actions()
        {
            var call = A.Fake<IInterceptedFakeObjectCall>();
            IFakeObjectCall passedCall = null;

            this.rule.UseApplicator(x => { });
            this.rule.Actions.Add(x => passedCall = x);

            this.rule.Apply(call);

            passedCall.Should().BeSameAs(call);
        }

        [Fact]
        public void Apply_should_call_CallBaseMethod_on_intercepted_call_when_CallBaseMethod_is_set_to_true()
        {
            var call = A.Fake<IInterceptedFakeObjectCall>();

            this.rule.UseApplicator(x => { });
            this.rule.CallBaseMethod = true;
            this.rule.Apply(call);

            A.CallTo(() => call.CallBaseMethod()).MustHaveHappened();
        }

        [Fact]
        public void Apply_should_set_ref_and_out_parameters_when_specified()
        {
            // Arrange
            this.rule.OutAndRefParametersValueProducer = x => new object[] { 1, "foo" };
            this.rule.UseApplicator(x => { });

            var call = A.Fake<IInterceptedFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod(nameof(IOutAndRef.OutAndRef)));

            // Act
            this.rule.Apply(call);

            A.CallTo(() => call.SetArgumentValue(1, 1)).MustHaveHappened();
            A.CallTo(() => call.SetArgumentValue(3, "foo")).MustHaveHappened();
        }

        [Fact]
        public void Apply_should_throw_when_OutAndRefParametersValues_length_differs_from_the_number_of_out_and_ref_parameters_in_the_call()
        {
            // Arrange
            this.rule.OutAndRefParametersValueProducer = x => new object[] { 1, "foo", "bar" };
            this.rule.UseApplicator(x => { });

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IOutAndRef).GetMethod(nameof(IOutAndRef.OutAndRef)));

            var exception = Record.Exception(() =>
                this.rule.Apply(call));

            exception.Should().BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.");
        }

        [Theory]
        [MemberData(nameof(DefaultReturnValueCases))]
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

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Apply_should_return_return_value_from_on_is_applicable_to_when_a_where_predicate_is_not_set(
            bool resultFromOnIsApplicableTo,
            bool expectedResult)
        {
            // Arrange
            this.rule.ReturnValueFromOnIsApplicableTo = resultFromOnIsApplicableTo;

            // Act
            var result = this.rule.IsApplicableTo(null);

            // Assert
            result.Should().Be(expectedResult);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "andon", Justification = "False positive")]
        [Fact]
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

        [Fact]
        public void Should_not_call_OnIsApplicableTo_when_a_where_predicate_returns_false()
        {
            // Arrange
            this.rule.ApplyWherePredicate(x => false, x => { });

            // Act
            this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>());

            // Assert
            this.rule.OnIsApplicableToWasCalled.Should().BeFalse();
        }

        [Fact]
        public void Apply_should_return_true_when_on_is_applicable_to_is_true_and_all_where_predicates_returns_true()
        {
            // Arrange
            this.rule.ReturnValueFromOnIsApplicableTo = true;

            this.rule.ApplyWherePredicate(x => true, x => { });
            this.rule.ApplyWherePredicate(x => true, x => { });

            // Act

            // Assert
            this.rule.IsApplicableTo(A.Dummy<IFakeObjectCall>()).Should().BeTrue();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void UseApplicator_should_not_be_callable_more_than_once()
        {
            this.rule.UseApplicator(x => { });

            var exception = Record.Exception(() => this.rule.UseApplicator(x => { }));
            exception.Should().BeAnExceptionOfType<InvalidOperationException>();
        }

        [Fact]
        public void OutAndRefParameterProducer_should_not_be_settable_more_than_once()
        {
            this.rule.OutAndRefParametersValueProducer = x => new object[0];

            var exception = Record.Exception(() => this.rule.OutAndRefParametersValueProducer = x => new object[] { "test" });
            exception.Should().BeAnExceptionOfType<InvalidOperationException>();
        }

        private class DummyableClass
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is loaded dynamically as an extension point")]
        private class DummyableClassFactory : DummyFactory<DummyableClass>
        {
            private static readonly DummyableClass Instance = new DummyableClass();

            protected override DummyableClass Create()
            {
                return Instance;
            }
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

            public bool OnIsApplicableToWasCalled { get; private set; }

            public override string DescriptionOfValidCall => this.DescriptionOfValidCallReturnValue;

            public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
            {
            }

            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                this.OnIsApplicableToWasCalled = true;
                return this.ReturnValueFromOnIsApplicableTo;
            }

            protected override BuildableCallRule CloneCallSpecificationCore()
            {
                return new TestableCallRule
                {
                    ReturnValueFromOnIsApplicableTo = this.ReturnValueFromOnIsApplicableTo,
                    DescriptionOfValidCallReturnValue = this.DescriptionOfValidCallReturnValue
                };
            }
        }
    }
}
