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
    using Xbehave;
    using Tests;
    using Record = FakeItEasy.Tests.TestHelpers.Record;

    public class CreationOptions
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterfaceThatWeWillAddAttributesTo
        {
        }

        [Scenario]
        public void when_ConfigureFake_is_used_to_configure_a_method(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(
                    options => options.ConfigureFake(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured value in fake options")));
            });

            "it should return the configured value during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options");
            });

            "it should return the configured value after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("configured value in fake options");
            });
        }

        [Scenario]
        public void when_ConfigureFake_is_used_to_configure_a_method_also_configured_by_a_FakeConfigurator(
            RobotRunsAmokEvent fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.ConfigureFake(
                        f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0))));
            });

            "it should use the configured behavior from the ConfigureFake"._(() =>
            {
                fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0));
            });
        }

        [Scenario]
        public void when_ConfigureFake_is_used_twice_to_configure_a_method(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("second value"))
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("first value").Once()));
            });

            "it should apply each configuration in turn"._(() =>
            {
                new[]
                {
                    fake.VirtualMethodValueDuringConstructorCall, fake.VirtualMethod(null)
                }
                .Should().Equal("first value", "second value");
            });
        }

        [Scenario]
        public void when_CallsBaseMethods_followed_by_ConfigureFake_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .CallsBaseMethods()
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake")));
            });

            "it should use behavior defined by ConfigureFake"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("a value from ConfigureFake");
            });
        }

        [Scenario]
        public void when_Strict_followed_by_ConfigureFake_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Strict()
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured value of strict fake")));
            });

            "it should return the configured value during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake");
            });

            "it should return the configured value after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("configured value of strict fake");
            });
        }

        [Scenario]
        public void when_Wrapping_followed_by_ConfigureFake_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Wrapping(new MakesVirtualCallInConstructor())
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured in test")));
            });

            "it should use the configured behavior during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test");
            });

            "it should use the configured behavior after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("configured in test");
            });
        }

        [Scenario]
        public void when_CallsBaseMethods_is_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods());
            });

            "it should call base method during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");
            });

            "it should call base method after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("implementation value");
            });
        }

        [Scenario]
        public void when_ConfigureFake_followed_by_CallsBaseMethods_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("a value from ConfigureFake"))
                    .CallsBaseMethods());
            });

            "it should call base method"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("implementation value");
            });
        }

        [Scenario]
        public void when_Strict_followed_by_CallsBaseMethods_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Strict()
                    .CallsBaseMethods());
            });

            "it should call base method"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("implementation value");
            });
        }

        [Scenario]
        public void when_Wrapping_followed_by_CallsBaseMethods_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value"))
                    .CallsBaseMethods());
            });

            "it should call base method"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("implementation value");
            });
        }

        [Scenario]
        public void when_Strict_is_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options.Strict());
            });

            "it should throw an exception from a method call during the constructor"._(() =>
            {
                fake.ExceptionFromVirtualMethodCallInConstructor
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
            });

            "it should throw an exception from a method call after the constructor"._(() =>
            {
                Record.Exception(() => fake.VirtualMethod("call outside constructor"))
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
            });
        }

        [Scenario]
        public void when_CallsBaseMethods_followed_by_Strict_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .CallsBaseMethods()
                    .Strict());
            });

            "it should throw an exception from a method call"._(() =>
            {
                Record.Exception(() => fake.VirtualMethod(null))
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>();
            });
        }
        [Scenario]
        public void when_ConfigureFake_followed_by_Strict_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured value of strict fake"))
                    .Strict());
            });

            "it should throw an exception from a method call"._(() =>
            {
                Record.Exception(() => fake.VirtualMethod(null))
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>();
            });
        }

        [Scenario]
        public void when_Wrapping_followed_by_Strict_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Wrapping(new MakesVirtualCallInConstructor())
                    .Strict());
            });

            "it should throw an exception from a method call"._(() =>
            {
                Record.Exception(() => fake.VirtualMethod(null))
                    .Should()
                    .BeAnExceptionOfType<ExpectationException>();
            });
        }

        [Scenario]
        public void when_Wrapping_is_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(
                    options => options.Wrapping(new MakesVirtualCallInConstructor()));
            });

            "it should delegate to the wrapped instance during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");
            });

            "it should delegate to the wrapped instance after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("implementation value");
            });
        }

        [Scenario]
        public void when_CallsBaseMethods_followed_by_Wrapping_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .CallsBaseMethods()
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));
            });

            "it should delegate to the wrapped instance"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("wrapped value");
            });
        }

        [Scenario]
        public void when_ConfigureFake_followed_by_Wrapping_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured in test"))
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));
            });

            "it should delegate to the wrapped instance during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("wrapped value");
            });

            "it should delegate to the wrapped instance after the constructor"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("wrapped value");
            });
        }

        [Scenario]
        public void when_Strict_followed_by_Wrapping_are_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Strict()
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("wrapped value")));
            });

            "it should delegate to the wrapped instance"._(() =>
            {
                fake.VirtualMethod(null).Should().Be("wrapped value");
            });
        }

        [Scenario]
        public void when_Wrapping_is_used_to_configure_a_fake_twice(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("first wrapped value"))
                    .Wrapping(new DerivedMakesVirtualCallInConstructor("second wrapped value")));
            });

            "it should delegate to the last wrapped instance"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be("second wrapped value");
            });
        }

        [Scenario]
        public void when_Wrapping_is_used_to_configure_a_fake_that_has_a_FakeConfigurator(
            RobotRunsAmokEvent fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<RobotRunsAmokEvent>(
                    options => options.Wrapping(new RobotRunsAmokEvent()));
            });

            "it should delegate to the wrapped object"._(() =>
            {
                fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp);
            });
        }

        [Scenario]
        public void when_Implements_is_used_to_configure_a_fake(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Implements(typeof(IDisposable)));
            });

            "it should produce a fake that implements the interface"._(() =>
            {
                fake.Should().BeAssignableTo<IDisposable>();
            });
        }

        [Scenario]
        public void when_Implements_is_used_to_configure_a_fake_twice()
        {
            MakesVirtualCallInConstructor fake = null;

            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .Implements(typeof(IComparable))
                    .Implements(typeof(ICloneable)));
            });

            "it should produce a fake that implements both interfaces"._(() =>
            {
                fake.Should().BeAssignableTo<IComparable>().And
                .BeAssignableTo<ICloneable>();
            });
        }


        [Scenario]
        public void when_WithAdditionalAttributes_is_used_to_configure_a_fake(
            IInterfaceThatWeWillAddAttributesTo fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
                var attribute = new CustomAttributeBuilder(constructor, new object[0]);
                var customAttributeBuilders = new List<CustomAttributeBuilder> { attribute };

                fake = A.Fake<IInterfaceThatWeWillAddAttributesTo>(options => options
                    .WithAdditionalAttributes(customAttributeBuilders));
            });

            "it should produce a fake that has the attribute"._(() =>
            {
                fake.GetType().GetCustomAttributes(typeof(ForTestAttribute), false).Should().HaveCount(1);
            });
        }

        [Scenario]
        public void when_WithAdditionalAttributes_is_used_to_configure_a_fake_with_a_null_set_of_attributes(
            Exception exception)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                exception = Record.Exception(() =>
                    A.Fake<IInterfaceThatWeWillAddAttributesTo>(options => options
                        .WithAdditionalAttributes(null)));
            });

            "it should throw an argument null exception"._(() =>
                               {
                                   exception.Should().BeAnExceptionOfType<ArgumentNullException>()
                                    .WithMessage("*customAttributeBuilders*");
                               });
        }

        [Scenario]
        public void when_WithAdditionalAttributes_is_used_to_configure_a_fake_twice(
            IInterfaceThatWeWillAddAttributesTo fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                var constructor1 = typeof(ScenarioAttribute).GetConstructor(new Type[0]);
                var attribute1 = new CustomAttributeBuilder(constructor1, new object[0]);
                var customAttributeBuilders1 = new List<CustomAttributeBuilder> { attribute1 };

                var constructor2 = typeof(ExampleAttribute).GetConstructor(new[] { typeof(object[]) });
                var attribute2 = new CustomAttributeBuilder(constructor2, new[] { new object[] { 1, null } });
                var constructor3 = typeof(DebuggerStepThroughAttribute).GetConstructor(new Type[0]);
                var attribute3 = new CustomAttributeBuilder(constructor3, new object[0]);
                var customAttributeBuilders2 = new List<CustomAttributeBuilder> { attribute2, attribute3 };

                fake = A.Fake<IInterfaceThatWeWillAddAttributesTo>(options => options
                    .WithAdditionalAttributes(customAttributeBuilders1)
                    .WithAdditionalAttributes(customAttributeBuilders2));
            });

            "it should produce a fake that has all of the attributes"._(() =>
            {
                fake.GetType().GetCustomAttributes(false)
                    .Select(a => a.GetType()).Should()
                    .Contain(typeof(ScenarioAttribute)).And
                    .Contain(typeof(ExampleAttribute)).And
                    .Contain(typeof(DebuggerStepThroughAttribute));
            });
        }

        [Scenario]
        public void when_WithArgumentsForConstructor_is_used_to_create_a_fake_with_a_list_of_arguments(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .WithArgumentsForConstructor(new object[]
                {
                    "prime argument", 2
                }));
            });

            "it should create a fake using the supplied arguments"._(() =>
            {
                fake.ConstructorArgument1.Should().Be("prime argument");
                fake.ConstructorArgument2.Should().Be(2);
            });
        }

        [Scenario]
        public void when_WithArgumentsForConstructor_is_used_to_create_a_fake_twice(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .WithArgumentsForConstructor(new object[]
                    {
                        "prime argument", 1
                    })
                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("secondary argument", 2)));
            });

            "it should create a fake using the last set of supplied arguments"._(() =>
            {
                fake.ConstructorArgument1.Should().Be("secondary argument");
                fake.ConstructorArgument2.Should().Be(2);
            });
        }

        [Scenario]
        public void when_WithArgumentsForConstructor_is_used_to_create_a_fake_with_an_example_constructor(
            MakesVirtualCallInConstructor fake)
        {
            "when ConfigureFake is used to configure a method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .WithArgumentsForConstructor(() => new MakesVirtualCallInConstructor("first argument", 9)));
            });

            "it should create a fake using the supplied arguments"._(() =>
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