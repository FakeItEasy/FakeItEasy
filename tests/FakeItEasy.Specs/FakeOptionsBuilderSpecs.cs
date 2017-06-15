namespace FakeItEasy.Specs
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class FakeOptionsBuilderSpecs
    {
        [Scenario]
        public static void FakeOptionsBuilderAppliesConfigureFake(
            RobotActivatedEvent fake)
        {
            "Given a type that has an implicit options builder defined"
                .See<RobotActivatedEvent>();

            "And the options builder updates the options to configure the fake"
                .See<DomainEventFakeOptionsBuilder>(_ => _.BuildOptions);

            "And the options builder updates the options to add an attribute"
                .See<DomainEventFakeOptionsBuilder>(_ => _.BuildOptions);

            "And the options builder updates the options to implement an interface specified by parameter"
                .See<DomainEventFakeOptionsBuilder>(_ => _.BuildOptions);

            "And the options builder updates the options to implement an interface specified by type parameter"
                .See<DomainEventFakeOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "Then the fake is configured"
                .x(() => fake.ID.Should().BeGreaterThan(0));

            "And the fake has the attribute"
                .x(() => fake.GetType().GetTypeInfo().GetCustomAttributes(inherit: false).Select(a => a.GetType())
                    .Should().Contain(typeof(ForTestAttribute)));

            "And the fake implements the interface specified by parameter"
                .x(() => fake.Should().BeAssignableTo<IDisposable>());

            "And the fake implements the interface specified by type parameter"
                .x(() => fake.Should().BeAssignableTo<IComparable>());
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderWrapping(
            WrapsAValidObject fake)
        {
            "Given a type that has an implicit options builder defined"
                .See<WrapsAValidObject>();

            "And the options builder updates the options to wrap an object"
                .See<WrapsAValidObjectOptionsBuilder>(_ => _.BuildOptions);

            "And calls to the wrapped object are to be recorded"
                .See<WrapsAValidObjectOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<WrapsAValidObject>());

            "And I call a method on the fake"
                .x(() => fake.AMethod());

            "Then the call is delegated to the wrapped object"
                .x(() => A.CallTo(() => WrapsAValidObjectOptionsBuilder.WrappedObject.AMethod()).MustHaveHappened());
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderWrappingNull(
            Exception exception)
        {
            "Given a type that has an implicit options builder defined"
                .See<WrapsNull>();

            "And the options builder updates the options to wrap null"
                .See<WrapsNullOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => exception = Record.Exception(() => A.Fake<WrapsNull>()));

            "Then an argument null exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderMakingStrict(
            Strict fake,
            Exception exception)
        {
            "Given a type that has an implicit options builder defined"
                .See<Strict>();

            "And the options builder updates the options to make it strict"
                .See<StrictOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<Strict>());

            "And I call a method on the fake"
                .x(() => exception = Record.Exception(() => fake.AMethod()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderCallsBaseMethods(
            CallsBaseMethods fake,
            string result)
        {
            "Given a type that has an implicit options builder defined"
                .See<CallsBaseMethods>();

            "And the options builder updates the options to call base methods"
                .See<CallsBaseMethodsOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<CallsBaseMethods>());

            "And I call a method on the fake"
                .x(() => result = fake.Name);

            "Then the base method is called"
                .x(() => result.Should().Be(typeof(CallsBaseMethods).Name));
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderConstructorArgumentsByList(
            ConstructorArgumentsSetByList fake)
        {
            "Given a type with a constructor that requires parameters"
                .See<ConstructorArgumentsSetByList>();

            "And the type has an implicit options builder defined"
                .See<ConstructorArgumentsSetByList>();

            "And the options builder updates the options to provide constructor arguments using a list"
                .See<ConstructorArgumentsSetByListOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<ConstructorArgumentsSetByList>());

            "Then it is created using the constructor arguments"
                .x(() => fake.ConstructorArgument.Should().Be(typeof(ConstructorArgumentsSetByListOptionsBuilder).Name));
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderConstructorArgumentsByConstructor(
            ConstructorArgumentsSetByConstructor fake)
        {
            "Given a type with a constructor that requires parameters"
                .See<ConstructorArgumentsSetByConstructor>();

            "And the type has an implicit options builder defined"
                .See<ConstructorArgumentsSetByConstructor>();

            "And the options builder updates the options to provide constructor arguments using a constructor"
                .See<ConstructorArgumentsSetByConstructorOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => fake = A.Fake<ConstructorArgumentsSetByConstructor>());

            "Then it is created using the constructor arguments"
                .x(() => fake.ConstructorArgument.Should().Be(typeof(ConstructorArgumentsSetByConstructorOptionsBuilder).Name));
        }

        [Scenario]
        public static void DefinedFakeOptionsBuilderConstructorArgumentsByConstructorForWrongType(
            Exception exception)
        {
            "Given a type with a constructor that requires parameters"
                .See<ConstructorArgumentsSetByConstructorForWrongType>();

            "And the type has an implicit options builder defined"
                .See<ConstructorArgumentsSetByConstructorForWrongType>();

            "And the options builder updates the options to provide constructor arguments using a constructor for the wrong type"
                .See<ConstructorArgumentsSetByConstructorForWrongTypeOptionsBuilder>(_ => _.BuildOptions);

            "When I create a fake of the type"
                .x(() => exception = Record.Exception(() => A.Fake<ConstructorArgumentsSetByConstructorForWrongType>()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                    .WithMessage("Supplied constructor is for type FakeItEasy.Specs.ConstructorArgumentsSetByConstructorForWrongType, but must be for FakeItEasy.Specs.ConstructorArgumentsSetByConstructor."));
        }

        [Scenario]
        public static void FakeOptionsBuilderPriority(
            RobotRunsAmokEvent fake)
        {
            "Given a type"
                .See<RobotRunsAmokEvent>();

            "And the type has an applicable implicit option builder defined"
                .See<RobotRunsAmokEventFakeOptionsBuilder>();

            "And the type has another applicable implicit option builder defined"
                .See<DomainEventFakeOptionsBuilder>();

            "When I create a fake of the type"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the configuration from the options builder with the higher priority is used"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public static void DuringConstruction(
            RobotRunsAmokEvent fake)
        {
            "Given a type with a parameterless constructor"
                .See<RobotRunsAmokEvent>();

            "And the constructor calls a virtual method"
                .See(() => new RobotRunsAmokEvent());

            "And the type has an implicit options builder defined"
                .See<RobotRunsAmokEventFakeOptionsBuilder>();

            "And the options builder updates the options to configure the fake"
                .See("RobotRunsAmokEventFakeOptionsBuilder.BuildOptions"); // it's protected

            "When I create a fake of the type"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the option builder's configuration is used during the constructor"
                .x(() => fake.Timestamp.Should().Be(RobotRunsAmokEventFakeOptionsBuilder.ConfiguredTimestamp));
        }

        [Scenario]
        public static void GenericFakeOptionsBuilderDefaultPriority(
            IFakeOptionsBuilder builder,
            Priority priority)
        {
            "Given a fake options builder that does not override priority"
                .x(() => builder = new SomeClassOptionsBuilder());

            "When I get the priority"
                .x(() => priority = builder.Priority);

            "Then it is the default priority"
                .x(() => priority.Should().Be(Priority.Default));
        }

        [Scenario]
        public static void GenericFakeOptionsBuilderBuildOptionsForMatchingType(
            SomeClass fake)
        {
            "Given a type with a parameterless constructor"
                .See<SomeClass>();

            "And the constructor calls a virtual method"
                .See(() => new SomeClass());

            "And the type has an implicit options builder defined"
                .See<SomeClassOptionsBuilder>();

            "And the options builder extends the generic base"
                .See<SomeClassOptionsBuilder>();

            "And the options builder updates the options to configure the fake"
                .See("SomeClassOptionsBuilder.BuildOptions"); // it's protected

            "When I create a fake of the type"
                .x(() => fake = A.Fake<SomeClass>());

            "Then the option builder's configuration is used during the constructor"
                .x(() => fake.IsConfigured.Should().BeTrue());
        }

        [Scenario]
        public static void GenericFakeOptionsBuilderBuildOptionsForDerivedType(
            SomeDerivedClass fake)
        {
            "Given a type with a parameterless constructor"
                .See<SomeDerivedClass>();

            "And the constructor calls a virtual method"
                .See(() => new SomeDerivedClass());

            "And the type has a parent"
                .See<SomeClass>();

            "And the parent has an implicit options builder defined"
                .See<SomeClassOptionsBuilder>();

            "And the options builder extends the generic base"
                .See<SomeClassOptionsBuilder>();

            "And the options builder updates the options to configure the fake"
                .See("SomeClassOptionsBuilder.BuildOptions"); // it's protected

            "When I create a fake of the type"
                .x(() => fake = A.Fake<SomeDerivedClass>());

            "Then the option builder's configuration is not used during the constructor"
                .x(() => fake.IsConfigured.Should().BeFalse());
        }

        [Scenario]
        public static void GenericFakeOptionsBuilderBuildOptionsForParentType(
            SomeParentClass fake)
        {
            "Given a type with a parameterless constructor"
                .See<SomeParentClass>();

            "And the constructor calls a virtual method"
                .See(() => new SomeParentClass());

            "And the type has a child"
                .See<SomeClass>();

            "And the child has an implicit options builder defined"
                .See<SomeClassOptionsBuilder>();

            "And the options builder extends the generic base"
                .See<SomeClassOptionsBuilder>();

            "And the options builder updates the options to configure the fake"
                .See("SomeClassOptionsBuilder.BuildOptions"); // it's protected

            "When I create a fake of the type"
                .x(() => fake = A.Fake<SomeParentClass>());

            "Then the option builder's configuration is not used during the constructor"
                .x(() => fake.IsConfigured.Should().BeFalse());
        }

        public class SomeParentClass
        {
            public bool IsConfigured { get; set; }
        }

        public class SomeClass : SomeParentClass
        {
        }

        public class SomeDerivedClass : SomeClass
        {
        }

        private class SomeClassOptionsBuilder : FakeOptionsBuilder<SomeClass>
        {
            protected override void BuildOptions(IFakeOptions<SomeClass> options)
            {
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                options.ConfigureFake(fake => fake.IsConfigured = true);
            }
        }
    }

    public abstract class ConventionBasedOptionsBuilder : IFakeOptionsBuilder
    {
        public Priority Priority => Priority.Default;

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type != null && type.FullName + "OptionsBuilder" == this.GetType().FullName;
        }

        public abstract void BuildOptions(Type typeOfFake, IFakeOptions options);
    }

    public class WrapsNull
    {
    }

    public class WrapsNullOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.Wrapping(null);
            }
        }
    }

    public class WrapsAValidObject
    {
        public virtual void AMethod()
        {
        }
    }

    public class AWrappedType : WrapsAValidObject
    {
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class WrapsAValidObjectOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public static AWrappedType WrappedObject { get; } = A.Fake<AWrappedType>();

        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.Wrapping(WrappedObject);
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public class Strict
    {
        public virtual void AMethod()
        {
        }
    }

    public class StrictOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.Strict();
            }
        }
    }

    public class CallsBaseMethods
    {
        public virtual string Name => typeof(CallsBaseMethods).Name;
    }

    public class CallsBaseMethodsOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.CallsBaseMethods();
            }
        }
    }

    public class ConstructorArgumentsSetByList
    {
        public ConstructorArgumentsSetByList(string argument)
        {
            this.ConstructorArgument = argument;
        }

        public string ConstructorArgument { get; }
    }

    public class ConstructorArgumentsSetByListOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.WithArgumentsForConstructor(new object[] { this.GetType().Name });
            }
        }
    }

    public class ConstructorArgumentsSetByConstructor
    {
        public ConstructorArgumentsSetByConstructor(string argument)
        {
            this.ConstructorArgument = argument;
        }

        public string ConstructorArgument { get; }
    }

    public class ConstructorArgumentsSetByConstructorOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.WithArgumentsForConstructor(() => new ConstructorArgumentsSetByConstructor(this.GetType().Name));
            }
        }
    }

    public class ConstructorArgumentsSetByConstructorForWrongType
    {
        public ConstructorArgumentsSetByConstructorForWrongType(string argument)
        {
            this.ConstructorArgument = argument;
        }

        public string ConstructorArgument { get; }
    }

    public class ConstructorArgumentsSetByConstructorForWrongTypeOptionsBuilder : ConventionBasedOptionsBuilder
    {
        public override void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.WithArgumentsForConstructor(() => new ConstructorArgumentsSetByConstructor(this.GetType().Name));
            }
        }
    }

    public class DomainEventFakeOptionsBuilder : IFakeOptionsBuilder
    {
        private int nextID = 1;

        public Priority Priority => Priority.Default;

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return typeof(DomainEvent).GetTypeInfo().IsAssignableFrom(type);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options == null)
            {
                return;
            }

            options.ConfigureFake(fake =>
            {
                var domainEvent = (DomainEvent)fake;
                domainEvent.ID = this.nextID++;
                domainEvent.Name = typeOfFake.Name;
            })
                .WithAttributes(() => new ForTestAttribute())
                .Implements(typeof(IDisposable))
                .Implements<IComparable>();
        }
    }

    public class RobotRunsAmokEventFakeOptionsBuilder : FakeOptionsBuilder<RobotRunsAmokEvent>
    {
        public static readonly DateTime ConfiguredTimestamp = new DateTime(1997, 8, 29, 2, 14, 03);

        public override Priority Priority => new Priority(5);

        protected override void BuildOptions(IFakeOptions<RobotRunsAmokEvent> options)
        {
            if (options == null)
            {
                return;
            }

            options.ConfigureFake(fake =>
            {
                var robotRunsAmokEvent = fake;
                A.CallTo(() => robotRunsAmokEvent.CalculateTimestamp())
                    .Returns(ConfiguredTimestamp);
                robotRunsAmokEvent.ID = -99;
            });
        }
    }
}
