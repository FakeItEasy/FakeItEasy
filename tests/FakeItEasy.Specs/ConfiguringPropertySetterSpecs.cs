namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ConfiguringPropertySetterSpecs
    {
        public interface IHaveInterestingProperties
        {
            int ReadWriteProperty { get; set; }

            int ReadOnlyProperty { get; }

            [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
            bool this[string genus, string species] { get; set; }

            int this[string commonName] { get; set; }

            string this[int count] { get; }

            int MethodThatLooksLikeAPropertyGetter();
        }

        [Scenario]
        public static void ConfiguringSetterWithNull(
            Exception exception)
        {
            "When assignment of a property is configured using a null expression"
                .x(() => exception = Record.Exception(() => A.CallToSet<int>(null)));

            "Then an argument null exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());

            "And the parameter name is propertySpecification"
                .x(() => exception.As<ArgumentNullException>().ParamName.Should().Be("propertySpecification"));
        }

        [Scenario]
        public static void ConfiguringNonConfigurableSetter(
            ClassWithInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a property that can't be configured"
                .x(() => subject = A.Fake<ClassWithInterestingProperties>());

            "When assignment of the property is configured"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => subject.NonConfigurableProperty).DoesNothing()));

            "Then a fake configuration exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>());
        }

        [Scenario]
        public static void ConfiguringSetterForReadOnlyProperty(
            IHaveInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a read-only property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When assignment of the property is configured"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => subject.ReadOnlyProperty).DoesNothing()));

            "Then an argument exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>());

            "And the exception message indicates that the property is read-only"
                .x(() => exception.Message.Should().Be(
                    $"The property {nameof(IHaveInterestingProperties.ReadOnlyProperty)} does not have a setter."));
        }

        [Scenario]
        public static void ConfiguringSetterViaMethod(
            IHaveInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When assignment of the property is configured using a method call expression"
                .x(() => exception = Record.Exception(() =>
                    A.CallToSet(() => subject.MethodThatLooksLikeAPropertyGetter()).DoesNothing()));

            "Then an argument exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>());

            "And the exception message indicates that the expression refers to an incorrect member type"
                .x(() => exception.Message.Should().EndWith("' must refer to a property or indexer getter, but doesn't."));
        }

        [Scenario]
        public static void ConfiguringSetterViaField(
            ClassWithInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<ClassWithInterestingProperties>());

            "When assignment of the property is configured using a field access expression"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => subject.Field).DoesNothing()));

            "Then an argument exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>());

            "And the exception message indicates that the expression refers to an incorrect member type"
                .x(() => exception.Message.Should().Be("The specified expression is not a method call or property getter."));
        }

        [Scenario]
        [Example(int.MinValue)]
        [Example(-42)]
        [Example(0)]
        [Example(42)]
        [Example(int.MaxValue)]
        public static void ConfiguringSetterForAnyValue(
            int value,
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for any value"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).Invokes(call => wasConfiguredBehaviorUsed = true));

            $"When I assign the property to {value}"
                .x(() => subject.ReadWriteProperty = value);

            "Then the configured behavior is used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        [Example(int.MinValue)]
        [Example(-42)]
        [Example(0)]
        [Example(42)]
        [Example(int.MaxValue)]
        public static void ConfiguringSetterToThrowForAnyValue(
            int value,
            IHaveInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured to throw an exception for any value"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).Throws(new InvalidOperationException("oops")));

            $"When I assign the property to {value}"
                .x(() => exception = Record.Exception(() => subject.ReadWriteProperty = value));

            "Then it throws the configured exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>().Which.Message.Should().Be("oops"));
        }

        [Scenario]
        [Example(int.MinValue)]
        [Example(-42)]
        [Example(0)]
        [Example(42)]
        [Example(int.MaxValue)]
        public static void ConfiguringSetterToDoNothingForAnyValue(
            int value,
            IHaveInterestingProperties subject)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured to do nothing for any value"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).DoesNothing());

            $"When I assign the property to {value}"
                .x(() => subject.ReadWriteProperty = value);

            "Then the default behavior is suppressed and the assigned value is not returned"
                .x(() => subject.ReadWriteProperty.Should().Be(0));
        }

        [Scenario]
        [Example(int.MinValue)]
        [Example(-42)]
        [Example(0)]
        [Example(42)]
        [Example(int.MaxValue)]
        public static void ConfiguringSetterToCallBaseMethodForAnyValue(
            int value,
            ClassWithInterestingProperties subject)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<ClassWithInterestingProperties>());

            "And assignment of the property is configured to call the base implementation for any value"
                .x(() => A.CallToSet(() => subject.ConfigurableProperty).CallsBaseMethod());

            $"When I assign the property to {value}"
                .x(() => subject.ConfigurableProperty = value);

            "Then the base implementation is called"
                .x(() => subject.WasBaseSetterCalled.Should().BeTrue());
        }

        [Scenario]
        public static void ConfigureIndexerWithWrongIndexes(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with an indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the indexer is configured"
                .x(() => A.CallToSet(() => subject["Choeropsis", "liberiensis"]).Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property using the wrong indexes"
                .x(() => subject["Eleoniscus", "helenae"] = false);

            "Then the configured behavior is not used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeFalse());
        }

        [Scenario]
        public static void ConfigureIndexerWithMatchingIndexes(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with an indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the indexer is configured"
                .x(() => A.CallToSet(() => subject["Choeropsis", "liberiensis"]).Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property using matching indexes"
                .x(() => subject["Choeropsis", "liberiensis"] = false);

            "Then the configured behavior is used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void ConfigureOverloadedIndexer(
            IHaveInterestingProperties subject,
            bool wasFirstConfiguredBehaviorUsed,
            bool wasSecondConfiguredBehaviorUsed)
        {
            "Given a Fake with an overloaded indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the indexer is configured for one signature"
                .x(() => A.CallToSet(() => subject["Choeropsis", "liberiensis"]).Invokes(call => wasFirstConfiguredBehaviorUsed = true));

            "And assignment of the indexer is configured for the other signature"
                .x(() => A.CallToSet(() => subject["Pygmy hippopotamus"]).Invokes(call => wasSecondConfiguredBehaviorUsed = true));

            "When I assign the property using one signature"
                .x(() => subject["Choeropsis", "liberiensis"] = true);

            "And I assign the property using the other signature"
                .x(() => subject["Pygmy hippopotamus"] = 4);

            "Then the configured behavior is used for the first signature"
                .x(() => wasFirstConfiguredBehaviorUsed.Should().BeTrue());

            "And the configured behavior is used for the second signature"
                .x(() => wasSecondConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void ConfigureReadOnlyIndexer(
            IHaveInterestingProperties subject,
            Exception exception)
        {
            "Given a Fake with a read-only indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When assignment of the indexer is configured"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => subject[7]).DoesNothing()));

            "Then an argument exception is thrown"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>());

            "And the exception message indicates that the property is read only"
                .x(() => exception.Message.Should().EndWith("refers to an indexed property that does not have a setter."));
        }

        [Scenario]
        public static void OverrideIndexerConfigurationWithWhenArgumentsMatch(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with an indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the indexer is configured using WhenArgumentsMatch"
                .x(() => A.CallToSet(() => subject["Choeropsis", "liberiensis"])
                    .WhenArgumentsMatch(arguments => arguments.Get<string>("genus") == "Canis")
                    .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property using indexes that satisfy the WhenArgumentsMatch"
                .x(() => subject["Canis", "lupus"] = false);

            "Then the configured behavior is used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void OverridePropertyValueConfigurationWithWhenArgumentsMatchAndCallWithGoodValue(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for value 3 using WhenArgumentsMatch"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty)
                    .WhenArgumentsMatch(arguments => arguments.Get<int>(0) == 3)
                    .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property value to 3"
                .x(() => subject.ReadWriteProperty = 3);

            "Then the configured behavior is not used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void OverridePropertyValueConfigurationWithWhenArgumentsMatchAndCallWithBadValue(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for value 3 using WhenArgumentsMatch"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty)
                    .WhenArgumentsMatch(arguments => arguments.Get<int>(0) == 3)
                    .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property value to 4"
                .x(() => subject.ReadWriteProperty = 4);

            "Then the configured behavior is not used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeFalse());
        }

        [Scenario]
        public static void OverrideIndexerConfigurationWithWithAnyArguments(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with an indexer"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the indexer is configured for specific arguments and overridden with WithAnyArguments"
                .x(() => A.CallToSet(() => subject["Choeropsis", "liberiensis"])
                    .WithAnyArguments()
                    .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property using the wrong indexes"
                .x(() => subject["Eleoniscus", "helenae"] = false);

            "Then the configured behavior is used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void ConfiguringSetterWithExactValueAndAssigningThatValue(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
               .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for a specific value"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).To(5)
                .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property to the matching value"
                .x(() => subject.ReadWriteProperty = 5);

            "Then the configured behavior is used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeTrue());
        }

        [Scenario]
        public static void ConfiguringSetterWithExactValueAndAssigningDifferentValue(
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
               .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for a specific value"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).To(5)
                .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property to a different value"
                .x(() => subject.ReadWriteProperty = -13);

            "Then the configured behavior is not used"
                .x(() => wasConfiguredBehaviorUsed.Should().BeFalse());
        }

        [Scenario]
        [Example(4, "not used")]
        [Example(5, "used")]
        public static void ConfiguringSetterWithValueSpecificationAndAssigningMatching(
            int actualValue,
            string fateOfConfiguredBehavior,
            IHaveInterestingProperties subject,
            bool wasConfiguredBehaviorUsed)
        {
            "Given a Fake with a property"
               .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And assignment of the property is configured for values greater than 4"
                .x(() => A.CallToSet(() => subject.ReadWriteProperty).To(() => A<int>.That.IsGreaterThan(4))
                    .Invokes(call => wasConfiguredBehaviorUsed = true));

            "When I assign the property to the value {0}"
                .x(() => subject.ReadWriteProperty = actualValue);

            "Then the configured behavior is {1}"
                .x(() => wasConfiguredBehaviorUsed.Should().Be(fateOfConfiguredBehavior == "used"));
        }

        public class ClassWithInterestingProperties
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for testing.")]
#pragma warning disable 649
            internal int Field;
#pragma warning restore 649

            public int NonConfigurableProperty { get; set; }

            public virtual int ConfigurableProperty
            {
                get { return 0; }
                set { this.WasBaseSetterCalled = true; }
            }

            public bool WasBaseSetterCalled { get; private set; }
        }
    }
}
