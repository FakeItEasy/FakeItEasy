namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeObjectCreatorTests
    {
        private readonly IProxyGenerator castleDynamicProxyGenerator;
        private readonly IProxyGenerator delegateProxyGenerator;
        private readonly FakeObjectCreator fakeObjectCreator;
        private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;

        public FakeObjectCreatorTests()
        {
            this.castleDynamicProxyGenerator = A.Fake<IProxyGenerator>();
            this.delegateProxyGenerator = A.Fake<IProxyGenerator>();
            string s;
            A.CallTo(() => this.castleDynamicProxyGenerator.CanGenerateProxy(A<Type>._, out s)).WithAnyArguments().Returns(true);
            this.fakeCallProcessorProviderFactory = A.Fake<FakeCallProcessorProvider.Factory>();

            this.fakeObjectCreator = new FakeObjectCreator(this.fakeCallProcessorProviderFactory, this.castleDynamicProxyGenerator, this.delegateProxyGenerator);
        }

        [Fact]
        public void Should_pass_fake_options_to_the_proxy_generator_and_return_the_fake_when_successful()
        {
            // Arrange
            var options = new ProxyOptions();

            var proxy = A.Fake<IFoo>();

            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));

            // Act
            var fakeCreationResult = this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>());

            // Assert
            fakeCreationResult.Result.Should().BeSameAs(proxy);

            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(
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
            this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>());

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(IFoo), options)).MustHaveHappened();

            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, fakeCallProcessorProvider))
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
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(TypeWithMultipleConstructors), options))
                .MustHaveHappened(3, Times.Exactly);
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
            var exception = Record.Exception(() => this.fakeObjectCreator.CreateFake(typeof(IFoo), options, new DummyCreationSession(), A.Dummy<IDummyValueResolver>()).Result);

            // Assert
            exception.Should().BeAnExceptionOfType<FakeCreationException>().Which
                .Message.Should().Contain("fail reason");
        }

        [Fact]
        public void Should_return_unsuccesful_result_with_no_fake_when_creation_fails()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            var createdFake = this.fakeObjectCreator.CreateFake(typeof(IFoo), new ProxyOptions(), new DummyCreationSession(), A.Dummy<IDummyValueResolver>());

            // Assert
            createdFake.WasSuccessful.Should().BeFalse();
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
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver);

            // Assert
            A.CallTo(() =>
                this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsNull(), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .MustHaveHappened()
                .Then(A.CallTo(() =>
                    this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                    .MustHaveHappened())
                .Then(A.CallTo(() =>
                    this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence("dummy"), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
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
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver);

            // Assert
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.Not.IsThisSequence(2, 2), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
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
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1, 1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(proxy));
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithMultipleConstructors), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .Returns(new ProxyGeneratorResult(new object()));

            // Act
            var fakeCreationResult = this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, new DummyCreationSession(), resolver);

            // Assert
            fakeCreationResult.Result.Should().BeSameAs(proxy);
        }

        [Fact]
        public void Should_not_try_constructor_where_not_all_arguments_are_resolved()
        {
            // Arrange
            var resolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(resolver, 1);
            StubResolverToFailForType<string>(resolver);

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithConstructorThatTakesDifferentTypes), new ProxyOptions(), new DummyCreationSession(), resolver);

            // Assert
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>.That.Not.IsNull(), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
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
            this.fakeObjectCreator.CreateFake(typeof(TypeWithProtectedConstructor), options, new DummyCreationSession(), resolver);

            // Assert
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(typeof(TypeWithProtectedConstructor), options.AdditionalInterfacesToImplement, A<IEnumerable<object>>.That.IsThisSequence(1), A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
                .MustHaveHappened();
        }

        private static void StubResolverToFailForType<T>(IDummyValueResolver resolver)
        {
            A.CallTo(() => resolver.TryResolveDummyValue(A<DummyCreationSession>._, typeof(T)))
                .Returns(CreationResult.FailedToCreateFake(typeof(T), "failed"));
        }

        private static void StubResolverWithDummyValue<T>(IDummyValueResolver resolver, T dummyValue)
        {
            A.CallTo(() => resolver.TryResolveDummyValue(A<DummyCreationSession>._, typeof(T)))
                .Returns(CreationResult.SuccessfullyCreated(dummyValue));
        }

        private void StubProxyGeneratorToFail(string failReason)
        {
            A.CallTo(() => this.castleDynamicProxyGenerator.GenerateProxy(A<Type>._, A<IEnumerable<Type>>._, A<IEnumerable<object>>._, A<IEnumerable<Expression<Func<Attribute>>>>._, A<IFakeCallProcessorProvider>._))
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
