namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public static class CreationOptionsSpecs
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
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that overrides the method"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured value in fake options")));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method returns the configured value during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options"));

            "And it returns the configured value after the constructor"
                .x(() => fake.VirtualMethod(null).Should().Be("configured value in fake options"));
        }

        [Scenario]
        public static void ConfigureFakeAfterConstruction(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            string result)
        {
            "Given an explicit options builder that overrides a method"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured value in fake options")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it uses the configured behavior"
                .x(() => result.Should().Be("configured value in fake options"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesFakeOptionsBuilder(
            RobotRunsAmokEvent fake,
            Action<IFakeOptions<RobotRunsAmokEvent>> optionsBuilder)
        {
            "Given a type with a parameterless constructor"
                .See<RobotRunsAmokEvent>();

            "And the constructor calls a virtual method"
                .See(() => new RobotRunsAmokEvent());

            "And an implicit options builder that overrides the method"
                .See<RobotRunsAmokEventFakeOptionsBuilder>();

            "And an explicit options builder that overrides the method"
                .x(() => optionsBuilder = options => options.ConfigureFake(
                    f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0))));

            "When I create a fake using the explicit options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it uses the explicitly configured behavior"
                .x(() => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0)));
        }

        [Scenario]
        public static void MultipleConfigureFakeConfigurations(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that overrides the method with changing behavior"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("second value"))
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("first value").Once()));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method uses the first behavior during the constructor "
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("first value"));

            "And the method uses the second behavior thereafter"
                .x(() => fake.VirtualMethod(null).Should().Be("second value"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesCallsBaseMethods(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake call base methods and then overrides a method"
                .x(() => optionsBuilder = options => options
                    .CallsBaseMethods()
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the overridden method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it uses the configured behavior"
                .x(() => result.Should().Be("a value from ConfigureFake"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesStrictDuringConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that makes a fake strict and then overrides the method"
                .x(() => optionsBuilder = options => options
                    .Strict()
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured value of strict fake")));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method uses the configured behavior during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesStrictAfterConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake strict and then overrides a method"
                .x(() => optionsBuilder = options => options
                    .Strict()
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured value of strict fake")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it uses the configured behavior"
                .x(() => result.Should().Be("configured value of strict fake"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesWrappingDuringConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that makes a fake wrap an object and then overrides the method"
                .x(() => optionsBuilder = options => options
                    .Wrapping(new MakesVirtualCallInConstructor())
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured in test")));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method uses the configured behavior during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test"));
        }

        [Scenario]
        public static void ConfigureFakeOverridesWrappingAfterConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake strict and then overrides a method"
                .x(() => optionsBuilder = options => options
                    .Wrapping(new MakesVirtualCallInConstructor())
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured value of strict fake")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it uses the configured behavior"
                .x(() => result.Should().Be("configured value of strict fake"));
        }

        [Scenario]
        public static void CallsBaseMethodsDuringConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that makes a fake call base methods"
                .x(() => optionsBuilder = options => options
                    .CallsBaseMethods());

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method calls the base method during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsAfterConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake call base methods"
                .x(() => optionsBuilder = options => options
                    .CallsBaseMethods());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it calls the base method"
                .x(() => result.Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesConfigureFake(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that overrides a method and then makes a fake call base methods"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))
                    .CallsBaseMethods());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it calls the base method"
                .x(() => result.Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesStrict(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake strict and then makes it call base methods"
                .x(() => optionsBuilder = options => options
                    .Strict()
                    .CallsBaseMethods());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it calls the base method"
                .x(() => result.Should().Be("implementation value"));
        }

        [Scenario]
        public static void CallsBaseMethodsOverridesWrapping(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake wrap an object and then makes it call base methods"
                .x(() => optionsBuilder = options => options
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))
                    .CallsBaseMethods());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it calls the base method"
                .x(() => result.Should().Be("implementation value"));
        }

        [Scenario]
        public static void StrictDuringConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that makes a fake strict"
                .x(() => optionsBuilder = options => options
                    .Strict());

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method throws an exception"
                .x(() => fake.ExceptionFromVirtualMethodCallInConstructor
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictAfterConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            Exception exception)
        {
            "Given an explicit options builder that makes a fake strict"
                .x(() => optionsBuilder = options => options
                    .Strict());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.VirtualMethod(null)));

            "Then it throws an exception"
                .x(() => exception
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictOverridesCallsBaseMethods(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            Exception exception)
        {
            "Given an explicit options builder that makes a fake call base methods and then be strict"
                .x(() => optionsBuilder = options => options
                    .CallsBaseMethods()
                    .Strict());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.VirtualMethod(null)));

            "Then it throws an exception"
                .x(() => exception
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictOverridesConfigureFake(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            Exception exception)
        {
            "Given an explicit options builder that overrides a method and then makes a fake strict"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured value of strict fake"))
                    .Strict());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.VirtualMethod(null)));

            "Then it throws an exception"
                .x(() => exception
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void StrictOverridesWrapping(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            Exception exception)
        {
            "Given an explicit options builder that makes a fake wrap an object and then be strict"
                .x(() => optionsBuilder = options => options
                    .Wrapping(new MakesVirtualCallInConstructor())
                    .Strict());

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.VirtualMethod(null)));

            "Then it throws an exception"
                .x(() => exception
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake."));
        }

        [Scenario]
        public static void WrappingDuringConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a parameterless constructor"
                .See<MakesVirtualCallInConstructor>();

            "And the constructor calls a virtual method"
                .See(() => new MakesVirtualCallInConstructor());

            "And an explicit options builder that makes a fake wrap an object"
                .x(() => optionsBuilder = options =>
                    options.Wrapping(new MakesVirtualCallInConstructor()));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method delegates to the wrapped instance during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value"));
        }

        [Scenario]
        public static void WrappingAfterConstruction(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake wrap an object"
                .x(() => optionsBuilder = options =>
                    options.Wrapping(new MakesVirtualCallInConstructor()));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it delegates to the wrapped instance"
                .x(() => result.Should().Be("implementation value"));
        }

        [Scenario]
        public static void WrappingOverridesCallsBaseMethods(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake call base methods and then wrap an object"
                .x(() => optionsBuilder = options => options
                    .CallsBaseMethods()
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it delegates to the wrapped instance"
                .x(() => result.Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesConfigureFake(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that overrides a method and then makes a fake wrap an object"
                .x(() => optionsBuilder = options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured in test"))
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it delegates to the wrapped instance"
                .x(() => result.Should().Be("wrapped value"));
        }

        [Scenario]
        public static void WrappingOverridesStrict(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake strict and then wrap an object"
                .x(() => optionsBuilder = options => options
                    .Strict()
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it delegates to the wrapped instance"
                .x(() => result.Should().Be("wrapped value"));
        }

        [Scenario]
        public static void MultipleWrappingConfigurations(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given an explicit options builder that makes a fake wrap two objects in turn"
                .x(() => optionsBuilder = options => options
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("first wrapped value"))
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("second wrapped value")));

            "And a fake created using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "When I call the method"
                .x(() => result = fake.VirtualMethod(null));

            "Then it delegates to the last wrapped instance"
                .x(() => result.Should().Be("second wrapped value"));
        }

        [Scenario]
        public static void WrappingViaImplicitOptionsBuilderOverridesConfigureFake(
            Action<IFakeOptions<RobotRunsAmokEvent>> optionsBuilder,
            RobotRunsAmokEvent fake)
        {
            "Given a type with a parameterless constructor"
                .See<RobotRunsAmokEvent>();

            "And the constructor calls a virtual method"
                .See(() => new RobotRunsAmokEvent());

            "And an implicit options builder that overrides the method"
                .See<RobotRunsAmokEventFakeOptionsBuilder>();

            "And an explicit options builder that makes a fake wrap an object"
                .x(() => optionsBuilder = options => options.Wrapping(new RobotRunsAmokEvent()));

            "When I create a fake using the explicit options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then the method delegates to the wrapped object"
                .x(() => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp));
        }

        [Scenario]
        public static void Implements(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given an explicit options builder that makes a fake implement an interface"
                .x(() => optionsBuilder = options => options
                    .Implements(typeof(IDisposable)));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it implements the interface"
                .x(() => fake.Should().BeAssignableTo<IDisposable>());
        }

        [Scenario]
        public static void ImplementsNonInterfaceType(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            Exception exception)
        {
            "Given an explicit options builder that makes a fake implement a non-interface type"
                .x(() => optionsBuilder = options => options
                    .Implements(typeof(string)));

            "When I create a fake using the options builder"
                .x(() => exception = Record.Exception(() => A.Fake(optionsBuilder)));

            "Then it throws an argument exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                    .WithMessage("*The specified type 'System.String' is not an interface*"));
        }

        [Scenario]
        public static void MultipleImplementsConfigurations(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given an explicit options builder that makes a fake implement two interfaces"
                .x(() => optionsBuilder = options => options
                    .Implements(typeof(IComparable))
                    .Implements(typeof(ICloneable)));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it implements the first interface"
                .x(() => fake.Should().BeAssignableTo<IComparable>());

            "And it implements the second interface"
                .x(() => fake.Should().BeAssignableTo<ICloneable>());
        }

        [Scenario]
        public static void WithAdditionalAttributes(
            IInterfaceThatWeWillAddAttributesTo1 fake,
            Action<IFakeOptions<IInterfaceThatWeWillAddAttributesTo1>> optionsBuilder)
        {
            "Given an explicit options builder that adds an attribute to a fake"
                .x(() =>
                {
                    var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
                    var attribute = new CustomAttributeBuilder(constructor, new object[0]);
                    var customAttributeBuilders = new List<CustomAttributeBuilder> { attribute };

                    optionsBuilder = options => options
                        .WithAdditionalAttributes(customAttributeBuilders);
                });

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it has the attribute"
                .x(() => fake.GetType().GetCustomAttributes(inherit: false).Select(a => a.GetType())
                    .Should().Contain(typeof(ForTestAttribute)));
        }

        [Scenario]
        public static void WithAdditionalAttributesAndNullSetOfAttributes(
            Action<IFakeOptions<IInterfaceThatWeWillAddAttributesTo2>> optionsBuilder,
            Exception exception)
        {
            "Given an explicit options builder that adds a null attribute to a fake"
                .x(() => optionsBuilder = options => options.WithAdditionalAttributes(null));

            "When I create a fake using the options builder"
                .x(() => exception = Record.Exception(() => A.Fake(optionsBuilder)));

            "Then it throws an argument null exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>()
                             .WithMessage("*customAttributeBuilders*"));
        }

        [Scenario]
        public static void MultipleWithAdditionalAttributesConfigurations(
            IInterfaceThatWeWillAddAttributesTo3 fake,
            Action<IFakeOptions<IInterfaceThatWeWillAddAttributesTo3>> optionsBuilder)
        {
            "Given an explicit options builder that adds multiple attributes to a fake"
                .x(() =>
                {
                    var constructor1 = typeof(ScenarioAttribute).GetConstructor(new Type[0]);
                    var attribute1 = new CustomAttributeBuilder(constructor1, new object[0]);
                    var customAttributeBuilders1 = new List<CustomAttributeBuilder> { attribute1 };

                    var constructor2 = typeof(ExampleAttribute).GetConstructor(new[] { typeof(object[]) });
                    var attribute2 = new CustomAttributeBuilder(constructor2, new object[] { new object[] { 1, null } });
                    var constructor3 = typeof(DebuggerStepThroughAttribute).GetConstructor(new Type[0]);
                    var attribute3 = new CustomAttributeBuilder(constructor3, new object[0]);
                    var customAttributeBuilders2 = new List<CustomAttributeBuilder> { attribute2, attribute3 };

                    optionsBuilder = options => options
                        .WithAdditionalAttributes(customAttributeBuilders1)
                        .WithAdditionalAttributes(customAttributeBuilders2);
                });

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it has all of the attributes"
                .x(() => fake.GetType().GetCustomAttributes(inherit: false)
                    .Select(a => a.GetType()).Should()
                    .Contain(typeof(ScenarioAttribute)).And
                    .Contain(typeof(ExampleAttribute)).And
                    .Contain(typeof(DebuggerStepThroughAttribute)));
        }

        [Scenario]
        public static void WithArgumentsForConstructor(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given a type with a constructor that requires parameters"
                .See<MakesVirtualCallInConstructor>();

            "And an explicit options builder that specifies the constructor arguments by array"
                .x(() => optionsBuilder = options => options
                    .WithArgumentsForConstructor(new object[]
                    {
                        "prime argument", 2
                    }));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it is constructed with the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("prime argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void MultipleWithArgumentsForConstructorConfigurations(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given a type with a constructor that requires parameters"
                .See<MakesVirtualCallInConstructor>();

            "And an explicit options builder that specifies the constructor arguments twice"
                .x(() => optionsBuilder = options => options
                    .WithArgumentsForConstructor(new object[]
                    {
                        "prime argument", 1
                    })
                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("secondary argument", 2)));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it is constructed with the last set of supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("secondary argument");
                        fake.ConstructorArgument2.Should().Be(2);
                    });
        }

        [Scenario]
        public static void WithArgumentsForConstructorWithExampleConstructor(
            MakesVirtualCallInConstructor fake,
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder)
        {
            "Given a type with a constructor that requires parameters"
                .See<MakesVirtualCallInConstructor>();

            "And an explicit options builder that specifies the constructor arguments by example"
                .x(() => optionsBuilder = options => options
                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("first argument", 9)));

            "When I create a fake using the options builder"
                .x(() => fake = A.Fake(optionsBuilder));

            "Then it is constructed with the supplied arguments"
                .x(() =>
                    {
                        fake.ConstructorArgument1.Should().Be("first argument");
                        fake.ConstructorArgument2.Should().Be(9);
                    });
        }

        [Scenario]
        public static void WithArgumentsForConstructorWithMethodThatIsNotAConstructor(
            Action<IFakeOptions<MakesVirtualCallInConstructor>> optionsBuilder,
            Exception exception)
        {
            "Given a type with a constructor that requires parameters"
                .See<MakesVirtualCallInConstructor>();

            "And an explicit options builder that specifies the constructor arguments using a method that is not a constructor"
                .x(() => optionsBuilder = options => options
                    .WithArgumentsForConstructor(() => A.Dummy<MakesVirtualCallInConstructor>()));

            "When I create a fake using the options builder"
                .x(() => exception = Record.Exception(() => A.Fake(optionsBuilder)));

            "Then fake creation fails"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                .WithMessage("*Only expression of the type ExpressionType.New (constructor calls) are accepted.*"));
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
