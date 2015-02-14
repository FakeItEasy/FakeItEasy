namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeObjectCreatorTests
    {
        private IProxyGenerator proxyGenerator;
        private FakeObjectCreator fakeObjectCreator;
        private IExceptionThrower thrower;
        private FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;

        [SetUp]
        public void Setup()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.thrower = A.Fake<IExceptionThrower>();
            this.fakeCallProcessorProviderFactory = A.Fake<FakeCallProcessorProvider.Factory>();

            this.fakeObjectCreator = new FakeObjectCreator(this.proxyGenerator, this.thrower, this.fakeCallProcessorProviderFactory);
        }

        [Test]
        public void Should_pass_fake_options_to_the_proxy_generator_and_return_the_fake_when_successful()
        {
            // Arrange
            var options = new FakeOptions
            {
                AdditionalInterfacesToImplement = new Type[] { },
                ArgumentsForConstructor = new object[] { },
                AdditionalAttributes = new CustomAttributeBuilder[] { }
            };

            var proxy = A.Fake<IFoo>();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(IFoo), options, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            createdFake.Should().BeSameAs(proxy);

            A.CallTo(() => this.proxyGenerator.GenerateProxy(
                    typeof(IFoo),
                    options.AdditionalInterfacesToImplement,
                    options.ArgumentsForConstructor,
                    options.AdditionalAttributes, 
                    A<IFakeCallProcessorProvider>._))
                .MustHaveHappened();
        }

        [Test]
        public void Should_use_new_fake_call_processor_for_the_proxy_generator()
        {
            // Arrange
            var options = new FakeOptions
            {
                FakeConfigurationActions = new Action<object>[] { },
            };

            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            A.CallTo(() => this.fakeCallProcessorProviderFactory(A<Type>._, A<FakeOptions>._)).Returns(fakeCallProcessorProvider);

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), options, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(IFoo), options)).MustHaveHappened();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<CustomAttributeBuilder>>._, fakeCallProcessorProvider))
                .MustHaveHappened();
        }

        [Test]
        public void Should_use_new_fake_call_processor_for_every_tried_constructor()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionWithDummyValue(session, 1);
            StubSessionWithDummyValue(session, "dummy");

            this.StubProxyGeneratorToFail();

            var options = new FakeOptions
            {
                FakeConfigurationActions = new Action<object>[] { },
            };

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, session, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(TypeWithMultipleConstructors), options))
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Test]
        public void Should_throw_when_generator_fails_and_arguments_for_constructor_are_specified()
        {
            // Arrange
            this.StubProxyGeneratorToFail("fail reason");

            var options = new FakeOptions
            {
                ArgumentsForConstructor = new object[] { "argument for constructor " }
            };

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), options, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: true);

            // Assert
            A.CallTo(() => this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeof(IFoo), "fail reason"))
                .MustHaveHappened();
        }

        [Test]
        public void Should_return_null_when_unsuccessful_and_throw_on_failure_is_false()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(IFoo), FakeOptions.Empty, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            createdFake.Should().BeNull();
        }

        [Test]
        public void Should_not_throw_when_unsuccessful_and_throw_on_failure_is_false()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(IFoo), FakeOptions.Empty, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            A.CallTo(this.thrower).MustNotHaveHappened();
        }

        [Test]
        public void Should_try_with_resolved_constructors_incorrect_order()
        {
            using (var scope = Fake.CreateScope())
            {
                // Arrange
                var session = A.Fake<IDummyValueCreationSession>();
                StubSessionWithDummyValue(session, 1);
                StubSessionWithDummyValue(session, "dummy");

                this.StubProxyGeneratorToFail();

                var options = new FakeOptions
                {
                    AdditionalInterfacesToImplement = new Type[] { }
                };

                // Act
                this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, session, throwOnFailure: false);

                // Assert
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsNull(), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                       .MustHaveHappened();
                    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                        .MustHaveHappened();
                    A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence("dummy"), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                        .MustHaveHappened();
                }
            }
        }

        [Test]
        public void Should_not_try_to_resolve_constructors_when_arguments_for_constructor_are_specified()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionWithDummyValue(session, 1);

            this.StubProxyGeneratorToFail();

            var options = new FakeOptions
            {
                AdditionalInterfacesToImplement = new Type[] { },
                ArgumentsForConstructor = new object[] { 2, 2 }
            };

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, session, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.Not.IsThisSequence(2, 2), A<IFakeCallProcessorProvider>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void Should_return_first_successfully_generated_proxy()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionWithDummyValue(session, 1);

            var options = new FakeOptions
            {
                AdditionalInterfacesToImplement = new Type[] { }
            };

            var proxy = A.Fake<IFoo>();

            this.StubProxyGeneratorToFail();
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(new object()));

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, session, throwOnFailure: false);

            // Assert
            createdFake.Should().BeSameAs(proxy);
        }

        [Test]
        public void Should_not_try_constructor_where_not_all_arguments_are_resolved()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionWithDummyValue(session, 1);
            StubSessionToFailForType<string>(session);

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithConstructorThatTakesDifferentTypes), FakeOptions.Empty, session, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>.That.Not.IsNull(), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void Should_try_protected_constructors()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionWithDummyValue(session, 1);
            this.StubProxyGeneratorToFail();

            var options = FakeOptions.Empty;

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithProtectedConstructor), options, session, throwOnFailure: false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(TypeWithProtectedConstructor), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
                .MustHaveHappened();
        }

        [Test]
        public void Should_throw_when_no_resolved_constructor_was_successfully_used()
        {
            // Arrange
            var session = A.Fake<IDummyValueCreationSession>();
            StubSessionToFailForType<int>(session);
            StubSessionWithDummyValue(session, "dummy");

            this.StubProxyGeneratorToFail("failed");

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), FakeOptions.Empty, session, throwOnFailure: true);

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

        private static void StubSessionToFailForType<T>(IDummyValueCreationSession session)
        {
            object outResult;
            A.CallTo(() => session.TryResolveDummyValue(typeof(T), out outResult))
                .Returns(false);
        }

        private static void StubSessionWithDummyValue<T>(IDummyValueCreationSession session, T dummyValue)
        {
            object outResult;
            A.CallTo(() => session.TryResolveDummyValue(typeof(T), out outResult))
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

                    foreach (var constructorPair in x.Zip(constructors))
                    {
                        if (!string.Equals(constructorPair.Item1.ReasonForFailure, constructorPair.Item2.ReasonForFailure))
                        {
                            return false;
                        }

                        if (constructorPair.Item1.Arguments.Length != constructorPair.Item2.Arguments.Length)
                        {
                            return false;
                        }

                        foreach (var argumentPair in constructorPair.Item1.Arguments.Zip(constructorPair.Item2.Arguments))
                        {
                            var isEqual =
                                object.Equals(argumentPair.Item1.ArgumentType, argumentPair.Item2.ArgumentType)
                                && object.Equals(argumentPair.Item1.ResolvedValue, argumentPair.Item2.ResolvedValue)
                                && argumentPair.Item1.WasResolved == argumentPair.Item2.WasResolved;

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
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<CustomAttributeBuilder>>._, A<IFakeCallProcessorProvider>._))
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
