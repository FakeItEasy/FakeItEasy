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
    using Machine.Specifications;
    using Tests;

    public class when_ConfigureFake_is_used_to_configure_a_method
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(
            options => options.ConfigureFake(
                f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured value in fake options")));

        It should_return_the_configured_value_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options");

        It should_return_the_configured_value_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured value in fake options");
    }

    public class when_ConfigureFake_is_used_to_configure_a_method_also_configured_by_a_FakeConfigurator
    {
        private static RobotRunsAmokEvent fake;

        Because of = () => fake = A.Fake<RobotRunsAmokEvent>(
            options => options.ConfigureFake(
                f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0))));

        It should_use_the_configured_behavior_from_the_ConfigureFake =
            () => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0));
    }

    public class when_ConfigureFake_is_used_twice_to_configure_a_method
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("second value"))
            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("first value").Once()));

        It should_apply_each_configuration_in_turn =
            () => new[]
            {
                fake.VirtualMethodValueDuringConstructorCall, fake.VirtualMethod(null)
            }
                .Should().Equal("first value", "second value");
    }

    public class when_CallsBaseMethods_followed_by_ConfigureFake_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .CallsBaseMethods()
            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake")));
            
        It should_use_behavior_defined_by_ConfigureFake =
            () => fake.VirtualMethod(null).Should().Be("a value from ConfigureFake");
    }

    public class when_Strict_followed_by_ConfigureFake_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Strict()
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured value of strict fake")));

        It should_return_the_configured_value_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake");

        It should_return_the_configured_value_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured value of strict fake");
    }

    public class when_Wrapping_followed_by_ConfigureFake_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Wrapping(new MakesVirtualCallInConstructor())
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured in test")));

        It should_use_the_configured_behavior_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test");

        It should_use_the_configured_behavior_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured in test");
    }

    public class when_CallsBaseMethods_is_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods());

        It should_call_base_method_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");

        It should_call_base_method_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_ConfigureFake_followed_by_CallsBaseMethods_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))
            .CallsBaseMethods());

        It should_call_base_method =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_Strict_followed_by_CallsBaseMethods_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .Strict()
            .CallsBaseMethods());

        It should_call_base_method =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_Wrapping_followed_by_CallsBaseMethods_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .Wrapping(new DerivedMakesVirtualCallInConstructor("derived value"))
            .CallsBaseMethods());

        It should_call_base_method =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_Strict_is_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.Strict());

        It should_throw_an_exception_from_a_method_call_during_the_constructor = () =>
        {
            fake.ExceptionFromVirtualMethodCallInConstructor
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
        };

        It should_throw_an_exception_from_a_method_call_after_the_constructor = () =>
        {
            Record.Exception(() => fake.VirtualMethod("call outside constructor"))
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
        };
    }

    public class when_CallsBaseMethods_followed_by_Strict_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options
            .CallsBaseMethods()
            .Strict());

        It should_throw_an_exception_from_a_method_call = () =>
        {
            Record.Exception(() => fake.VirtualMethod(null))
                .Should()
                .BeAnExceptionOfType<ExpectationException>();
        };
    }

    public class when_ConfigureFake_followed_by_Strict_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured value of strict fake"))
                .Strict());

        It should_throw_an_exception_from_a_method_call = () =>
        {
            Record.Exception(() => fake.VirtualMethod(null))
                .Should()
                .BeAnExceptionOfType<ExpectationException>();
        };
    }

    public class when_Wrapping_followed_by_Strict_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Wrapping(new MakesVirtualCallInConstructor())
                .Strict());

        It should_throw_an_exception_from_a_method_call = () =>
        {
            Record.Exception(() => fake.VirtualMethod(null))
                .Should()
                .BeAnExceptionOfType<ExpectationException>();
        };
    }

    public class when_Wrapping_is_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(
                options => options.Wrapping(new MakesVirtualCallInConstructor()));

        It should_delegate_to_the_wrapped_instance_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");

        It should_delegate_to_the_wrapped_instance_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_CallsBaseMethods_followed_by_Wrapping_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .CallsBaseMethods()
                .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));

        It should_call_the_base_method = () =>
            fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_ConfigureFake_followed_by_Wrapping_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured in test"))
                .Wrapping(new MakesVirtualCallInConstructor()));

        It should_use_the_configured_behavior_during_the_constructor = () =>
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test");

        It should_use_the_configured_behavior_after_the_constructor = () =>
            fake.VirtualMethod(null).Should().Be("configured in test");
    }

    public class when_Strict_followed_by_Wrapping_are_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Strict()
                .Wrapping(new DerivedMakesVirtualCallInConstructor("derived value")));

        It should_throw_an_exception_from_the_method_call = () =>
            fake.ExceptionFromVirtualMethodCallInConstructor
                .Should()
                .BeAnExceptionOfType<ExpectationException>();
    }

    public class when_Wrapping_is_used_to_configure_a_fake_twice
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("first wrapped value"))
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("second wrapped value")));

        It should_delegate_to_the_last_wrapped_instance = () =>
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("second wrapped value");
    }

    public class when_Wrapping_is_used_to_configure_a_fake_that_has_a_FakeConfigurator
    {
        private static RobotRunsAmokEvent fake;

        Because of = () =>
            fake = A.Fake<RobotRunsAmokEvent>(
                options => options.Wrapping(new RobotRunsAmokEvent()));

        It should_delegate_to_the_wrapped_object =
            () => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp);
    }

    public class when_Implements_is_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Implements(typeof(IDisposable)));

        It should_produce_a_fake_that_implements_the_interface =
            () => fake.Should().BeAssignableTo<IDisposable>();
    }

    public class when_Implements_is_used_to_configure_a_fake_twice
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Implements(typeof(IComparable))
                .Implements(typeof(ICloneable)));

        It should_produce_a_fake_that_implements_both_interfaces =
            () => fake.Should().BeAssignableTo<IComparable>().And
                .BeAssignableTo<ICloneable>();
    }

    public class when_WithAdditionalAttributes_is_used_to_configure_a_fake
    {
        private static IInterfaceThatWeWillAddAttributesTo fake;

        Because of = () =>
        {
            var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
            var attribute = new CustomAttributeBuilder(constructor, new object[0]);
            var customAttributeBuilders = new List<CustomAttributeBuilder> { attribute };

            fake = A.Fake<IInterfaceThatWeWillAddAttributesTo>(options => options
                .WithAdditionalAttributes(customAttributeBuilders));
        };

        It should_produce_a_fake_that_has_the_attribute =
            () => fake.GetType().GetCustomAttributes(typeof(ForTestAttribute), false).Should().HaveCount(1);

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo
        {
        }
    }

    public class when_WithAdditionalAttributes_is_used_to_configure_a_fake_twice
    {
        private static IInterfaceThatWeWillAddAttributesTo fake;

        Because of = () =>
        {
            var constructor1 = typeof(SubjectAttribute).GetConstructor(new[] { typeof(Type) });
            var attribute1 = new CustomAttributeBuilder(constructor1, new[] { typeof(string) });
            var customAttributeBuilders1 = new List<CustomAttributeBuilder> { attribute1 };

            var constructor2 = typeof(IgnoreAttribute).GetConstructor(new[] { typeof(string) });
            var attribute2 = new CustomAttributeBuilder(constructor2, new[] { "ignore it" });
            var constructor3 = typeof(DebuggerStepThroughAttribute).GetConstructor(new Type[0]);
            var attribute3 = new CustomAttributeBuilder(constructor3, new object[0]);
            var customAttributeBuilders2 = new List<CustomAttributeBuilder> { attribute2, attribute3 };

            fake = A.Fake<IInterfaceThatWeWillAddAttributesTo>(options => options
                .WithAdditionalAttributes(customAttributeBuilders1)
                .WithAdditionalAttributes(customAttributeBuilders2));
        };

        It should_produce_a_fake_that_has_the_last_set_of_attributes =
            () => fake.GetType().GetCustomAttributes(false)
                .Select(a => a.GetType()).Should()
                .Contain(typeof(IgnoreAttribute)).And
                .Contain(typeof(DebuggerStepThroughAttribute));

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo
        {
        }
    }

    public class when_WithArgumentsForConstructor_is_used_to_create_a_fake_with_a_list_of_arguments
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .WithArgumentsForConstructor(new object[]
                {
                    "prime argument", 2
                }));

        It should_create_a_fake_using_the_supplied_arguments =
            () =>
            {
                fake.ConstructorArgument1.Should().Be("prime argument");
                fake.ConstructorArgument2.Should().Be(2);
            };
    }

    public class when_WithArgumentsForConstructor_is_used_to_create_a_fake_twice
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .WithArgumentsForConstructor(new object[]
                {
                    "prime argument", 1
                })
                .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("secondary argument", 2)));

        It should_create_a_fake_using_the_last_set_of_supplied_arguments =
            () =>
            {
                fake.ConstructorArgument1.Should().Be("secondary argument");
                fake.ConstructorArgument2.Should().Be(2);
            };
    }

    public class when_WithArgumentsForConstructor_is_used_to_create_a_fake_with_an_example_constructor
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("first argument", 9)));

        It should_create_a_fake_using_the_supplied_arguments = () =>
        {
            fake.ConstructorArgument1.Should().Be("first argument");
            fake.ConstructorArgument2.Should().Be(9);
        };
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