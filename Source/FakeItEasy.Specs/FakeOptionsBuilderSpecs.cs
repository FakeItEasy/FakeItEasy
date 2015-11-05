namespace FakeItEasy.Specs
{
    using System;
    using System.Reflection.Emit;
    using Core;
    using Creation;
    using FluentAssertions;
    using Tests;
    using Tests.TestHelpers;
    using Xbehave;

    public class FakeOptionsBuilderSpecs
    {
        [Scenario]
        public void DefinedFakeOptionsBuilder(
            RobotActivatedEvent fake)
        {
            "When a fake is created for a type that has an options builder defined"
                .x(() => fake = A.Fake<RobotActivatedEvent>());

            "Then the builder will apply the fake options builder"
                .x(() => fake.ID.Should().BeGreaterThan(0));

            "And it will be passed the fake type"
                .x(() => fake.Name.Should().Be(typeof(RobotActivatedEvent).Name));

            "And it will add its extra attributes"
                .x(() => fake.GetType().GetCustomAttributes(typeof(ForTestAttribute), true).Should().HaveCount(1));

            "And it will make the fake implement its extra interface specified by parameter"
                .x(() => fake.Should().BeAssignableTo<IDisposable>());

            "And it will make the fake implement its extra interface specified by type parameter"
                .x(() => fake.Should().BeAssignableTo<ICloneable>());
        }

        [Scenario]
        public void DefinedFakeOptionsBuilderWrapping(
            WrapsAValidObject fake)
        {
            "When a fake is created for a type that has an options builder defined"
                .x(() => fake = A.Fake<WrapsAValidObject>());

            "Then the fake will wrap the configured target"
                .x(() =>
                {
                    fake.AMethod();
                    A.CallTo(() => WrapsAValidObjectOptionsBuilder.WrappedObject.AMethod()).MustHaveHappened();
                });
        }

        [Scenario]
        public void DefinedFakeOptionsBuilderWrappingNull(
            Exception exception)
        {
            "When a fake is created for a type that has an options builder defined that wraps null"
                .x(() => exception = Record.Exception(() => A.Fake<WrapsNull>()));

            "Then an argument null exception will be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());
        }

        [Scenario]
        public void FakeOptionsBuilderPriority(
            RobotRunsAmokEvent fake)
        {
            "When a fake is created for a type that has two applicable fake options builders"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the builder with the higher priority will build options for the fake"
                .x(() => fake.ID.Should().Be(-99));
        }

        [Scenario]
        public void DuringConstruction(
            RobotRunsAmokEvent fake)
        {
            "When a fake is created for a type that has an options builder defined"
                .x(() => fake = A.Fake<RobotRunsAmokEvent>());

            "Then the builder will build options to be used during the fake's constructor"
                .x(() => fake.Timestamp.Should().Be(RobotRunsAmokEventFakeOptionsBuilder.ConfiguredTimestamp));
        }

        [Scenario]
        public void GenericFakeOptionsBuilderDefaultPriority(
            IFakeOptionsBuilder builder,
            int priority)
        {
            "Given an options builder that extends the generic base"
                .x(() => builder = new SomeClassOptionsBuilder());

            "When the default priority is fetched"
                .x(() => priority = builder.Priority);

            "Then it should be 0"
                .x(() => priority.Should().Be(0));
        }

        [Scenario]
        public void GenericFakeOptionsBuilderBuildOptionsForMatchingType(
            SomeParentClass fake)
        {
            "When we create a fake of a type that has an options builder extending the generic base"
                .x(() => fake = A.Fake<SomeClass>());

            "Then the generic build options method will be used to configure it"
                .x(() => fake.IsConfigured.Should().BeTrue());
        }

        [Scenario]
        public void GenericFakeOptionsBuilderBuildOptionsForDerivedType(
            SomeParentClass fake)
        {
            "When we create a fake of a type whose parent has an options builder extending the generic base"
                .x(() => fake = A.Fake<SomeDerivedClass>());

            "Then the generic build options method will be not used to configure it"
                .x(() => fake.IsConfigured.Should().BeFalse());
        }

        [Scenario]
        public void GenericFakeOptionsBuilderBuildOptionsForParentType(
            SomeParentClass fake)
        {
            "When we create a fake of a type whose child has an options builder extending the generic base"
                .x(() => fake = A.Fake<SomeParentClass>());

            "Then the generic build options method will be not used to configure it"
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
                    throw new ArgumentNullException("options");
                }

                options.ConfigureFake(fake => fake.IsConfigured = true);
            }
        }
    }

    public class WrapsNull
    {
    }

    public class WrapsNullOptionsBuilder : IFakeOptionsBuilder
    {
        public int Priority
        {
            get { return 0; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(WrapsNull);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
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

    public class WrapsAValidObjectOptionsBuilder : IFakeOptionsBuilder
    {
        private static readonly AWrappedType WrappedFake = A.Fake<AWrappedType>();

        public static AWrappedType WrappedObject
        {
            get { return WrappedFake; }
        }

        public int Priority
        {
            get { return 0; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(WrapsAValidObject);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options != null)
            {
                options.Wrapping(WrappedObject);
            }
        }
    }

    public class DomainEventFakeOptionsBuilder : IFakeOptionsBuilder
    {
        private int nextID = 1;

        public int Priority
        {
            get { return -1; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return typeof(DomainEvent).IsAssignableFrom(type);
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
                .WithAdditionalAttributes(new[] { CreateCustomAttributeBuilder() })
                .Implements(typeof(IDisposable))
                .Implements<ICloneable>();
        }

        private static CustomAttributeBuilder CreateCustomAttributeBuilder()
        {
            var constructor = typeof(ForTestAttribute).GetConstructor(new Type[0]);
            return new CustomAttributeBuilder(constructor, new object[0]);
        }
    }

    public class RobotRunsAmokEventFakeOptionsBuilder : IFakeOptionsBuilder
    {
        public static readonly DateTime ConfiguredTimestamp = new DateTime(1997, 8, 29, 2, 14, 03);

        public int Priority
        {
            get { return 0; }
        }

        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(RobotRunsAmokEvent);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (options == null)
            {
                return;
            }

            options.ConfigureFake(fake =>
            {
                var robotRunsAmokEvent = (RobotRunsAmokEvent)fake;
                A.CallTo(() => robotRunsAmokEvent.CalculateTimestamp())
                    .Returns(ConfiguredTimestamp);
                robotRunsAmokEvent.ID = -99;
            });
        }
    }
}