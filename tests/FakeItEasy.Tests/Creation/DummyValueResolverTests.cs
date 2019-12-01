namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DummyValueResolverTests
    {
        private readonly IFakeObjectCreator fakeObjectCreator;

        public DummyValueResolverTests()
        {
            this.fakeObjectCreator = A.Fake<IFakeObjectCreator>();
            A.CallTo(() => this.fakeObjectCreator.CreateFakeWithoutLoopDetection(
                    A<Type>._,
                    A<IProxyOptions>._,
                    A<IDummyValueResolver>._,
                    A<LoopDetectingResolutionContext>._))
                .ReturnsLazily((
                        Type type,
                        IProxyOptions proxyOptions,
                        IDummyValueResolver resolver,
                        LoopDetectingResolutionContext resolutionContext) =>
                    CreationResult.FailedToCreateDummy(type, string.Empty));
        }

        public static IEnumerable<object?[]> DummiesInContainer()
        {
            return TestCases.FromObject(
                "dummy value",
                Task.FromResult(7),
                Task.WhenAll()); // creates a completed Task
        }

        [Theory]
        [MemberData(nameof(DummiesInContainer))]
        public void Should_return_dummy_from_container_when_available(object dummyForContainer)
        {
            Guard.AgainstNull(dummyForContainer, nameof(dummyForContainer));

            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakes(dummyForContainer),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(dummyForContainer.GetType(), new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().BeSameAs(dummyForContainer);
        }

        [Fact]
        public void Should_return_failed_creation_result_when_type_cannot_be_created()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(TypeThatCanNotBeInstantiated),
                new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeFalse();
        }

        [Fact]
        public void Should_return_fake_when_it_can_be_created()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            this.StubFakeObjectCreatorWithValue(fake);
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(typeof(IFoo), new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().BeSameAs(fake);
        }

        [Fact]
        public void Should_return_default_value_when_type_is_value_type()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(typeof(int), new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().Be(0);
        }

        [Fact]
        public void Should_be_able_to_create_class_with_default_constructor()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(ClassWithDefaultConstructor),
                new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().BeOfType<ClassWithDefaultConstructor>();
        }

        [Fact]
        public void Should_be_able_to_create_class_with_resolvable_constructor_arguments()
        {
            // Arrange
            this.StubFakeObjectCreatorWithValue(A.Fake<IFoo>());
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakes("dummy string"),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(TypeWithResolvableConstructorArguments<string, IFoo>),
                new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().BeOfType<TypeWithResolvableConstructorArguments<string, IFoo>>();
        }

        [Fact]
        public void Should_not_be_able_to_create_class_with_circular_dependencies()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(TypeWithCircularDependency),
                new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_resolve_same_type_twice_when_successful()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakes("dummy value"),
                this.fakeObjectCreator);

            resolver.TryResolveDummyValue(typeof(string), new LoopDetectingResolutionContext());

            // Act
            var result = resolver.TryResolveDummyValue(typeof(string), new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            result.Result.Should().Be("dummy value");
        }

        [Fact]
        public void Should_return_failed_result_when_default_constructor_throws()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(TypeWithDefaultConstructorThatThrows),
                new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeFalse();
        }

        [Fact]
        public void Should_favor_the_widest_constructor_when_activating()
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakes("dummy value"),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(
                typeof(TypeWithMultipleConstructorsOfDifferentWidth),
                new LoopDetectingResolutionContext());

            // Assert
            var dummy = result.Result;
            dummy.Should().BeOfType<TypeWithMultipleConstructorsOfDifferentWidth>();
            ((TypeWithMultipleConstructorsOfDifferentWidth)dummy!).WidestConstructorWasCalled.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(void))]
        [InlineData(typeof(Func<int>))]
        [InlineData(typeof(Action))]
        public void Should_return_failed_result_for_restricted_types(Type restrictedType)
        {
            // Arrange
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(restrictedType, new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_create_lazy_wrapper()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            this.StubFakeObjectCreatorWithValue(fake);
            var resolver = new DummyValueResolver(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            var result = resolver.TryResolveDummyValue(typeof(Lazy<IFoo>), new LoopDetectingResolutionContext());

            // Assert
            result.WasSuccessful.Should().BeTrue();
            var dummy = result.Result;
            dummy.Should().BeOfType<Lazy<IFoo>>();
            ((Lazy<IFoo>)dummy!).Value.Should().BeSameAs(fake);
        }

        private void StubFakeObjectCreatorWithValue<T>(T value)
        {
            A.CallTo(() => this.fakeObjectCreator.CreateFakeWithoutLoopDetection(
                    typeof(T),
                    A<IProxyOptions>._,
                    A<IDummyValueResolver>._,
                    A<LoopDetectingResolutionContext>._))
                .Returns(CreationResult.SuccessfullyCreated(value));
        }

        private DynamicDummyFactory CreateDummyFactoryThatMakes(object value)
        {
            return new DynamicDummyFactory(new[] { new FixedDummyFactory(value) });
        }

        private DynamicDummyFactory CreateDummyFactoryThatMakesNoDummy()
        {
            return new DynamicDummyFactory(Array.Empty<IDummyFactory>());
        }

        public class ClassWithDefaultConstructor
        {
        }

        public class TypeWithResolvableConstructorArguments<T1, T2>
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithResolvableConstructorArguments(T1 argument1, T2 argument2)
            {
            }
        }

        public class TypeWithCircularDependency
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "circular", Justification = "Required for testing.")]
            public TypeWithCircularDependency(TypeWithCircularDependency circular)
            {
            }
        }

        public class TypeWithMultipleConstructorsOfDifferentWidth
        {
            public TypeWithMultipleConstructorsOfDifferentWidth()
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1, string argument2)
            {
                this.WidestConstructorWasCalled = true;
            }

            public bool WidestConstructorWasCalled { get; }
        }

        internal class FixedDummyFactory : IDummyFactory
        {
            private readonly object dummy;

            public FixedDummyFactory(object dummy)
            {
                this.dummy = dummy;
            }

            public Priority Priority => Priority.Default;

            public bool CanCreate(Type type)
            {
                return type == this.dummy.GetType();
            }

            public object Create(Type type)
            {
                return this.dummy;
            }
        }

        private static class TypeThatCanNotBeInstantiated
        {
        }

        private class TypeWithDefaultConstructorThatThrows
        {
            public TypeWithDefaultConstructorThatThrows()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
