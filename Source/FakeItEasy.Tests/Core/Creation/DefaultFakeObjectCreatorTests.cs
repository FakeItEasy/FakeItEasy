namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeObjectCreatorTests
    {
        private IProxyGenerator2 proxyGenerator;
        private FakeObjectCreator generator;

        [SetUp]
        public void SetUp()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator2>();

            this.generator = new FakeObjectCreator(this.proxyGenerator);
        }

        [Test]
        public void Should_return_fake_from_proxy_generator_when_successful()
        {
            // Arrange
            var proxy = A.Fake<IFoo>();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(
                    typeof(IFoo), A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument))
                .Returns(new ProxyGeneratorResult(proxy, A.Dummy<ICallInterceptedEventRaiser>()));

            // Act
            var fake = this.generator.CreateFake(typeof(IFoo), FakeOptions.Empty, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            Assert.That(fake, Is.SameAs(proxy));
        }

        [Test]
        public void Should_pass_arguments_for_constructor_to_proxy_generator_when_specified()
        {
            // Arrange
            this.StubProxyGeneratorToSucceed();

            var options = new FakeOptions
            {
                ArgumentsForConstructor = new object[] { "constructor argument" }
            };

            // Act
            this.generator.CreateFake(typeof(IFoo), options, A.Dummy<IDummyValueCreationSession>(), false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.That.IsThisSequence("constructor argument").Argument))
                .MustHaveHappened();
        }

        [Test]
        public void Should_pass_additional_interfaces_to_proxy_generator()
        {
            // Arrange
            this.StubProxyGeneratorToSucceed();

            var options = new FakeOptions
            {
                AdditionalInterfacesToImplement = new[] { typeof(IFormatProvider) }
            };

            // Act
            this.generator.CreateFake(typeof(IFoo), options, A.Dummy<IDummyValueCreationSession>(), false);

            // Assert
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.That.IsThisSequence(typeof(IFormatProvider)).Argument, A<IEnumerable<object>>.Ignored.Argument))
                .MustHaveHappened();
        }

        [Test]
        public void Should_throw_exception_when_proxy_can_not_be_generated_and_throw_on_failure_is_true()
        {
            // Arrange
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument))
                .Returns(new ProxyGeneratorResult("fail"));

            // Act, Assert
            Assert.Throws<FakeCreationException>(() =>
                this.generator.CreateFake(typeof(IFoo), FakeOptions.Empty, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: true));
        }

        [Test]
        public void Should_return_null_when_proxy_can_not_be_generated_and_should_not_throw_on_failure()
        {
            // Arrange
            this.StubProxyGeneratorToFail();

            // Act
            var result = this.generator.CreateFake(typeof(IFoo), FakeOptions.Empty, A.Dummy<IDummyValueCreationSession>(), throwOnFailure: false);

            // Assert
            Assert.That(result, Is.Null);
        }

        [TestCase(typeof(TypeWithNoDefaultConstructor))]
        [TestCase(typeof(TypeWithProtectedConstructor))]
        public void Should_try_with_dummy_values_for_constructor_when_empty_arguments_fails(Type typeOfFake)
        {
            // Arrange
            var proxy = new object();

            var session = A.Fake<IDummyValueCreationSession>();

            this.StubSessionWithDummyValue(session, "foo");
            this.StubSessionWithDummyValue(session, 10);

            this.StubProxyGeneratorToFail();
            A.CallTo(() => this.proxyGenerator.GenerateProxy(
                    typeOfFake,
                    A<IEnumerable<Type>>.Ignored.Argument,
                    A<IEnumerable<object>>.That.IsThisSequence("foo", 10).Argument))
                .Returns(new ProxyGeneratorResult(proxy, A.Dummy<ICallInterceptedEventRaiser>()));

            // Act
            var result = this.generator.CreateFake(typeOfFake, FakeOptions.Empty, session, throwOnFailure: false);

            // Assert
            Assert.That(result, Is.SameAs(proxy));
        }

        private void StubSessionWithDummyValue<T>(IDummyValueCreationSession session, T value)
        {
            object output;
            A.CallTo(() => session.TryResolveDummyValue(typeof(T), out output))
                .Returns(true)
                .AssignsOutAndRefParameters(value);
        }

        private void StubProxyGeneratorToFail()
        {
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument))
                .Returns(new ProxyGeneratorResult("fail"));
        }

        private void StubProxyGeneratorToSucceed()
        {
            A.CallTo(() => this.proxyGenerator.GenerateProxy(A<Type>.Ignored, A<IEnumerable<Type>>.Ignored.Argument, A<IEnumerable<object>>.Ignored.Argument))
                .Returns(new ProxyGeneratorResult(new object(), A.Dummy<ICallInterceptedEventRaiser>()));
        }

        public class TypeWithNoDefaultConstructor
        {
            public TypeWithNoDefaultConstructor(string argument1, int argument2)
            {

            }
        }

        public class TypeWithProtectedConstructor
        {
            protected TypeWithProtectedConstructor(string argument1, int argument2)
            {

            }
        }
    }
}
