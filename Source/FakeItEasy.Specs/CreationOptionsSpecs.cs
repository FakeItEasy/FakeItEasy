namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Tests;
    using Xbehave;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required for testing.")]
    public class CreationOptionsSpecs
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo1
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo2
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo3
        {
        }

        [Scenario]
        public static void ConfigureFakeDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "When configuring a fake during construction"
                .x(() =>
                    {
                        fake = A.Fake<MakesVirtualCallInConstructor>(
                            options => options.ConfigureFake(
                                f => A.CallTo(() => f.VirtualMethod(A<string>._))
                            .Returns("configured value in fake options")));
                    });

            "Then it should return the configured value during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options"));

            "And it should return the configured value after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured value in fake options"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesFakeConfigurator(
            RobotRunsAmokEvent fake)
        {
            "When configuring a fake to configure a method already configured by a fake configurator"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.ConfigureFake(
                        f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0)))));

            "Then it should use the behavior configured in the creation options"
                .x(() => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0)));
        }

        [Scenario]
        public static void MultipleConfigureFakeConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "When configuring a fake multiple times"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("second value"))
                            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("first value").Once())));

            "Then it should apply each configuration in turn"
                .x(() => new[]
                             {
                                 fake.VirtualMethodValueDuringConstructorCall, fake.VirtualMethod(null)
                             }
                             .Should().Equal("first value", "second value"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying that a fake calls base methods followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))));

            "Then it should behave as defined by the explicit configuration"
                .x(() => fake.VirtualMethod(null).Should().Be("a value from ConfigureFake"));
        }

        [Scenario]
        public static void StrictCombinedWithConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying a strict fake followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured value of strict fake"))));

            "Then it should return the configured value during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake"));

            "And it should return the configured value after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured value of strict fake"));
        }

        [Scenario]
        public static void WrappingCombinedWithConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is configured to wrap an object followed by explicit configuration"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new MakesVirtualCallInConstructor())
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured in test"))));

            "Then it should use the configured behavior during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test"));

            "And it should use the configured behavior after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured in test"));
        }

        [Scenario]
        public static void CallsBaseMethodsDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying that a fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods()));

            "Then it should call the base method during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));

            "And it should call base method after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "When explicit configuration is followed by specifying that the fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))
                                    .CallsBaseMethods()));

            "Then it should call the base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesStrict(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying a strict fake followed by specifying that the fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .CallsBaseMethods()));

            "Then it should call the base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesWrapping(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is configured to wrap an object followed by specifying that the fake calls base methods"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))
                                    .CallsBaseMethods()));

            "Then it should call the base method"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void StrictDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying a strict fake"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.Strict()));

            "Then it should throw an exception from a method call during the constructor"
                .x(() => fake.ExceptionFromVirtualMethodCallInConstructor
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>()
                             .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));

            "And it should throw an exception from a method call after the constructor"
                .x(() => Record.Exception(() => fake.VirtualMethod("call outside constructor"))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>()
                             .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying that a fake calls base methods followed by specifying that the fake is strict"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .Strict()));

            "Then it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void StrictOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "When explicit fake configuration is followed by specifying that the fake is strict"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured value of strict fake"))
                                            .Strict()));

            "Then it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void StrictOverridesWrapping(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is configured to wrap an object followed by specifying that the fake is strict"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new MakesVirtualCallInConstructor())
                                        .Strict()));

            "Then it should throw an exception from a method call"
                .x(() => Record.Exception(() => fake.VirtualMethod(null))
                             .Should()
                             .BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void WrappingDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is configured to wrap an object"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(
                    options => options.Wrapping(new MakesVirtualCallInConstructor())));

            "Then it should delegate to the wrapped instance during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));

            "And then it should delegate to the wrapped instance after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("implementation value"));
        }

        [Scenario]
        public static void WrappingOverridesCallsBaseMethods(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying that a fake calls base methods followed by wrapping an object"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .CallsBaseMethods()
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "Then it should delegate to the wrapped instance"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesConfigureFake(
            MakesVirtualCallInConstructor fake)
        {
            "When explicit configuration is followed by wrapping an object"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                                        .Returns("configured in test"))
                                            .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "Then it should delegate to the wrapped instance during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("wrapped value"));

            "And then it should delegate to the wrapped instance after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesStrict(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying a strict fake followed by configuring it to wrap an object"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Strict()
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))));

            "Then it should delegate to the wrapped instance"
                .x(() => fake.VirtualMethod(null).Should().Be("wrapped value"));
        }

        [Scenario]
        public static void MultipleWrappingConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is configured to wrap two different objects"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("first wrapped value"))
                                    .Wrapping(new DerivedMakesVirtualCallInConstructor("second wrapped value"))));

            "Then it should delegate to the last wrapped instance"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("second wrapped value"));
        }

        [Scenario]
        public static void Wrapping(
            RobotRunsAmokEvent fake)
        {
            "When a fake that has a fake configurator is configured to wrap an object"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.Wrapping(new RobotRunsAmokEvent())));

            "Then it should delegate to the wrapped object"
                .x(() => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp));
        }

        [Scenario]
        public static void Implements(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is built to implement an interface"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                     .Implements(typeof(IDisposable))));

            "Then it should implement the interface"
                .x(() => fake.Should().BeAssignableTo<IDisposable>());
        }

        [Scenario]
        public static void ImplementsNonInterfaceType(
            Exception exception)
        {
            "When a fake is built to implement a non-interface type"
                .x(() => exception = Record.Exception(() => A.Fake<MakesVirtualCallInConstructor>(options => options
                                                                .Implements(typeof(MakesVirtualCallInConstructor)))));

            "Then it should throw an argument exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                             .WithMessage("*The specified type 'FakeItEasy.Specs.MakesVirtualCallInConstructor' is not an interface*"));
        }

        [Scenario]
        public static void MultipleImplementsConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "When a fake is built to implement two interfaces in turn"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .Implements(typeof(IComparable))
                                    .Implements(typeof(ICloneable))));

            "Then it should implement both interfaces"
                .x(() => fake.Should().BeAssignableTo<IComparable>().And
                             .BeAssignableTo<ICloneable>());
        }

        [Scenario]
        public static void WithAdditionalAttributes(
            IInterfaceThatWeWillAddAttributesTo1 fake)
        {
            "When a fake is built with an additional attribute"
                .x(() =>
                    {
                        var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
                        var attribute = new CustomAttributeBuilder(constructor, new object[0]);
                        var customAttributeBuilders = new List<CustomAttributeBuilder> { attribute };

                        fake = A.Fake<IInterfaceThatWeWillAddAttributesTo1>(options => options
                            .WithAdditionalAttributes(customAttributeBuilders));
                    });

            "Then it should have the attribute"
                .x(() => fake.GetType().GetCustomAttributes(typeof(ForTestAttribute), false).Should().HaveCount(1));
        }

        [Scenario]
        public static void WithAdditionalAttributesAndNullSetOfAttributes(
            Exception exception)
        {
            "When a fake is built with a null set of additional attributes"
                .x(() => exception = Record.Exception(() => A.Fake<IInterfaceThatWeWillAddAttributesTo2>(options => options.WithAdditionalAttributes(null))));

            "Then it should throw an argument null exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>()
                             .WithMessage("*customAttributeBuilders*"));
        }

        [Scenario]
        public static void MultipleWithAdditionalAttributesConfigurations(
            IInterfaceThatWeWillAddAttributesTo3 fake)
        {
            "When a fake is built with two sets of additional attributes"
                .x(() =>
                    {
                        var constructor1 = typeof(ScenarioAttribute).GetConstructor(new Type[0]);
                        var attribute1 = new CustomAttributeBuilder(constructor1, new object[0]);
                        var customAttributeBuilders1 = new List<CustomAttributeBuilder> { attribute1 };

                        var constructor2 = typeof(ExampleAttribute).GetConstructor(new[] { typeof(object[]) });
                        var attribute2 = new CustomAttributeBuilder(constructor2, new[] { new object[] { 1, null } });
                        var constructor3 = typeof(DebuggerStepThroughAttribute).GetConstructor(new Type[0]);
                        var attribute3 = new CustomAttributeBuilder(constructor3, new object[0]);
                        var customAttributeBuilders2 = new List<CustomAttributeBuilder> { attribute2, attribute3 };

                        fake = A.Fake<IInterfaceThatWeWillAddAttributesTo3>(options => options
                            .WithAdditionalAttributes(customAttributeBuilders1)
                            .WithAdditionalAttributes(customAttributeBuilders2));
                    });

            "Then it should have all of the attributes"
                .x(() => fake.GetType().GetCustomAttributes(false)
                             .Select(a => a.GetType()).Should()
                             .Contain(typeof(ScenarioAttribute)).And
                             .Contain(typeof(ExampleAttribute)).And
                             .Contain(typeof(DebuggerStepThroughAttribute)));
        }

        [Scenario]
        public static void WithArgumentsForConstructor(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying fake constructor arguments with a list of arguments"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(new object[]
                                                                    {
                                                                        "prime argument", 2
                                                                    })));

            "Then it should be constructed with the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("prime argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void MultipleWithArgumentsForConstructorConfigurations(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying fake constructor arguments twice"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(new object[]
                                                                    {
                                                                        "prime argument", 1
                                                                    })
                                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("secondary argument", 2))));

            "Then the fake should be constructed with the last set of supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("secondary argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void WithArgumentsForConstructorWithExampleConstructor(
            MakesVirtualCallInConstructor fake)
        {
            "When specifying fake constructor arguments with an example constructor"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("first argument", 9))));

            "Then it should be constructed with the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("first argument");
                        fake.ConstructorArgument2.Should().Be(9);
                    });
        }
    }

    public class DerivedMakesVirtualCallInConstructor : MakesVirtualCallInConstructor
    {
        private readonly string virtualMethodReturnValue;

        public DerivedMakesVirtualCallInConstructor(string virtualMethodReturnValue)
        {
            this.virtualMethodReturnValue = virtualMethodReturnValue;
        }

        public override string VirtualMethod(string parameter)
        {
            return this.virtualMethodReturnValue;
        }
    }
}