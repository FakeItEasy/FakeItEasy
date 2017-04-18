namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeObjectCreatorTests
    {
        private readonly IProxyGenerator proxyGenerator;
        private readonly FakeObjectCreator fakeObjectCreator;
        private readonly IExceptionThrower thrower;
        private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;

        public FakeObjectCreatorTests()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.thrower = A.Fake<IExceptionThrower>();
            this.fakeCallProcessorProviderFactory = A.Fake<FakeCallProcessorProvider.Factory>();

            this.fakeObjectCreator = new FakeObjectCreator(this.proxyGenerator, this.thrower, this.fakeCallProcessorProviderFactory);
        }

        [Fact]
        public void Should_pass_fake_options_to_the_proxy_generator_and_return_the_fake_when_successful()
        {
            // Arrange
            var options = new ProxyOptions();

            var proxy = A.Fake<IFoo>();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>(), throwOnFailure: false);

            // Assert
            createdFake.Should().BeSameAs(proxy);

            A.CallTo(() => this.proxyGenerator.GenerateProxy(
                    typeof(IFoo),
                    options.AdditionalInterfacesToImplement,
                    options.ArgumentsForConstructor,
                    options.Attributes,
                    A<IFakeCallProcessorProvider>._))
                .MustHaveHappened();
        }

        [Fact]
        public void Should_use_new_fake_call_processor_for_the_proxy_generator()
        {
            // Arrange
            var options = new ProxyOptions();

            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            A.CallTo(() => this.fakeCallProcessorProviderFactory(A<Type>._, A<IProxyOptions>._)).Returns(fakeCallProcessorProvider);

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>(), throwOnFailure: false);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(IFoo), options)).MustHaveHappened();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, fakeCallProcessorProvider))
                .MustHaveHappened();
        }

        [Fact]
        public void Should_use_new_fake_call_processor_for_every_tried_constructor()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);
            StubResolverWithDummyValue(resolver, "dummy");

            this.StubProxyGeneratorToFail();

            var options = new ProxyOptions();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(TypeWithMultipleConstructors), options))
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void Should_throw_when_generator_fails_and_arguments_for_constructor_are_specified()
        {
            // Arrange
            this.StubProxyGeneratorToFail("fail reason");

            var options = new ProxyOptions
            {
                ArgumentsForConstructor = new object[] { "argument for constructor " }
            };

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>(), throwOnFailure: true);

            // Assert
            A.CallTo(() => this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeof(IFoo), "fail reason"))
                .MustHaveHappened();
        }

        [Fact]
        public void Should_return_null_when_unsuccessful_and_throw_on_failure_is_false()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(IFoo), new ProxyOptions(), new DummyCreationSession(), A.Dummy<IDummyValueResolver>(), throwOnFailure: false);

            // Assert
            createdFake.Should().BeNull();
        }

        [Fact]
        public void Should_not_throw_when_unsuccessful_and_throw_on_failure_is_false()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), new ProxyOptions(), new DummyCreationSession(), A.Dummy<IDummyValueResolver>(), throwOnFailure: false);

            // Assert
            A.CallTo(this.thrower).MustNotHaveHappened();
        }

        [Fact]
        public void Should_try_with_resolved_constructors_in_the_correct_order()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);
            StubResolverWithDummyValue(resolver, "dummy");

            this.StubProxyGeneratorToFail();

            var options = new ProxyOptions();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            A.CallTo(() =>
                this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsNull(), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .MustHaveHappened()
                .Then(A.CallTo(() =>
                    this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                    .MustHaveHappened())
                .Then(A.CallTo(() =>
                    this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence("dummy"), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                    .MustHaveHappened());
        }

        [Fact]
        public void Should_not_try_to_resolve_constructors_when_arguments_for_constructor_are_specified()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);

            this.StubProxyGeneratorToFail();

            var options = new ProxyOptions
            {
                ArgumentsForConstructor = new object[] { 2, 2 }
            };

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.Not.IsThisSequence(2, 2), A<IFakeCallProcessorProvider>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void Should_return_first_successfully_generated_proxy()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);

            var options = new ProxyOptions();

            var proxy = A.Fake<IFoo>();

            this.StubProxyGeneratorToFail();
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(new object()));

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            createdFake.Should().BeSameAs(proxy);
        }

        [Fact]
        public void Should_not_try_constructor_where_not_all_arguments_are_resolved()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);
            StubResolverToFailForType<string>(resolver);

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithConstructorThatTakesDifferentTypes), new ProxyOptions(), new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>.That.Not.IsNull(), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void Should_try_protected_constructors()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);
            this.StubProxyGeneratorToFail();

            var options = new ProxyOptions();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithProtectedConstructor), options, new DummyCreationSession(), resolver, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithProtectedConstructor), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .MustHaveHappened();
        }

        [Fact]
        public void Should_throw_when_no_resolved_constructor_was_successfully_used()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverToFailForType<int>(resolver);
            StubResolverWithDummyValue(resolver, "dummy");

            this.StubProxyGeneratorToFail("failed");

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), new ProxyOptions(), new DummyCreationSession(), resolver, throwOnFailure: true);

            // Assert
            var expectedConstructors = new[]
            {
                new ResolvedConstructor
                {
                    Arguments = new[]
                    {
                        new ResolvedArgument
                        {
                            ArgumentType = typeof(int),
                            ResolvedValue = null,
                            WasResolved = false
                        },
                        new ResolvedArgument
                        {
                            ArgumentType = typeof(int),
                            ResolvedValue = null,
                            WasResolved = false
                        }
                    }
                },
                new ResolvedConstructor
                {
                    ReasonForFailure = "failed",
                    Arguments = new[]
                    {
                        new ResolvedArgument
                        {
                            ArgumentType = typeof(string),
                            ResolvedValue = "dummy",
                            WasResolved = true
                        }
                    }
                }
            };

            A.CallTo(() => this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(typeof(TypeWithMultipleConstructors), "failed", this.ConstructorsEquivalentTo(expectedConstructors)))
                .MustHaveHappened();
        }

        private static void StubResolverToFailForType<T>(IDummyValueResolver resolver)
        {
            object outResult;
            A.CallTo(() => resolver.TryResolveDummyValue(A<DummyCreationSession>._, typeof(T), out outResult))
                .Returns(false);
        }

        private static void StubResolverWithDummyValue<T>(IDummyValueResolver resolver, T dummyValue)
        {
            object outResult;
            A.CallTo(() => resolver.TryResolveDummyValue(A<DummyCreationSession>._, typeof(T), out outResult))
                .Returns(true)
                .AssignsOutAndRefParameters(dummyValue);
        }

        private IEnumerable<ResolvedConstructor> ConstructorsEquivalentTo(IEnumerable<ResolvedConstructor> constructors)
        {
            return A<IEnumerable<ResolvedConstructor>>.That.Matches(
                x =>
                {
                    if (x.Count() != constructors.Count())
                    {
                        return false;
                    }

                    foreach (var constructorPair in x.Zip(constructors, (constructor1, constructor2) => new { Constructor1 = constructor1, Constructor2 = constructor2 }))
                    {
                        if (!string.Equals(constructorPair.Constructor1.ReasonForFailure, constructorPair.Constructor2.ReasonForFailure))
                        {
                            return false;
                        }

                        if (constructorPair.Constructor1.Arguments.Length != constructorPair.Constructor2.Arguments.Length)
                        {
                            return false;
                        }

                        foreach (var argumentPair in constructorPair.Constructor1.Arguments.Zip(constructorPair.Constructor2.Arguments, (argument1, argument2) => new { Argument1 = argument1, Argument2 = argument2 }))
                        {
                            var isEqual =
                                object.Equals(argumentPair.Argument1.ArgumentType, argumentPair.Argument2.ArgumentType)
                                && object.Equals(argumentPair.Argument1.ResolvedValue, argumentPair.Argument2.ResolvedValue)
                                && argumentPair.Argument1.WasResolved == argumentPair.Argument2.WasResolved;

                            if (!isEqual)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                },
                "Matching constructor");
        }

        private void StubProxyGeneratorToFail(string failReason)
        {
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(failReason));
        }

        private void StubProxyGeneratorToFail()
        {
            this.StubProxyGeneratorToFail("failed");
        }

        public class TypeWithMultipleConstructors
        {
            public TypeWithMultipleConstructors()
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            public TypeWithMultipleConstructors(string argument1)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithMultipleConstructors(int argument1, int argument2)
            {
            }
        }

        public class TypeWithConstructorThatTakesDifferentTypes
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithConstructorThatTakesDifferentTypes(int argument1, string argument2)
            {
            }
        }

        public class TypeWithProtectedConstructor
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument", Justification = "Required for testing.")]
            protected TypeWithProtectedConstructor(int argument)
            {
            }
        }
    }
}
