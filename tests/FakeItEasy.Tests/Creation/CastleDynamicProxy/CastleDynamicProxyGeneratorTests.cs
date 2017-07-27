namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class CastleDynamicProxyGeneratorTests
    {
        private readonly CastleDynamicProxyGenerator generator;
        private readonly CastleDynamicProxyInterceptionValidator interceptionValidator;

        public CastleDynamicProxyGeneratorTests()
        {
            this.interceptionValidator = A.Fake<CastleDynamicProxyInterceptionValidator>();

            this.generator = new CastleDynamicProxyGenerator(this.interceptionValidator);
        }

        public interface IInterfaceType
        {
            void Foo(int argument1, int argument2);
        }

        public static IEnumerable<object[]> SupportedTypes()
        {
            return TestCases.FromObject(
                typeof(IInterfaceType),
                typeof(AbstractClass),
                typeof(ClassWithProtectedConstructor),
                typeof(ClassWithInternalConstructor),
                typeof(InternalType));
        }

        public static IEnumerable<object[]> NotSupportedTypes()
        {
            return TestCases.FromObject(
                typeof(int),
                typeof(ClassWithPrivateConstructor));
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_return_proxy(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_return_proxy_that_is_of_the_specified_type(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull()
                .And.Subject.Should().Match(p => typeOfProxy.IsInstanceOfType(p));
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_true(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(NotSupportedTypes))]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_false_when_proxy_cannot_be_generated(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_delegate_to_fake_call_processor_when_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            IInterceptedFakeObjectCall interceptedFakeObjectCall = null;

            var fakeCallProcessorProvider = CreateFakeCallProcessorProvider(c => interceptedFakeObjectCall = c);

            var result = this.generator.GenerateProxy(typeThatImplementsInterfaceType, Enumerable.Empty<Type>(), null, fakeCallProcessorProvider);

            var proxy = (IInterfaceType)result.GeneratedProxy;

            // Act
            proxy.Foo(1, 2);

            // Assert
            interceptedFakeObjectCall.Should().NotBeNull();
            interceptedFakeObjectCall.Arguments.Should().BeEquivalentTo(1, 2);
            interceptedFakeObjectCall.Method.Name.Should().Be(
                typeof(IInterfaceType).GetMethod(nameof(IInterfaceType.Foo)).Name);
            interceptedFakeObjectCall.FakedObject.Should().BeSameAs(proxy);
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_ensure_fake_call_processor_is_initialized_but_not_fetched_when_no_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            // Act
            this.generator.GenerateProxy(typeThatImplementsInterfaceType, Enumerable.Empty<Type>(), null, fakeCallProcessorProvider);

            // Assert
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => fakeCallProcessorProvider.EnsureInitialized(A<object>._)).MustHaveHappened();
        }

#if FEATURE_BINARY_SERIALIZATION
        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Serialized_proxies_should_deserialize_to_an_object(Type typeOfProxy)
        {
            // Arrange
            // Here we can't use A.Dummy<IFakeCallProcessorProvider>() because the EnsureInitialized() call within GenerateProxy()
            // triggers the Castle issue #65 (https://github.com/castleproject/Core/issues/65)
            var result = this.generator.GenerateProxy(typeOfProxy, new Type[] { }, null, new SerializableFakeCallProcessorProvider());
            var proxy = result.GeneratedProxy;

            // Act
            var deserializedProxy = BinarySerializationHelper.SerializeAndDeserialize(proxy);

            // Assert
            deserializedProxy.Should().NotBeNull();
        }
#endif

        [Fact]
        [UsingCulture("en-US")]
        public void Should_specify_that_value_types_cannot_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(int), Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("The type of proxy must be an interface or a class but it was System.Int32.");
        }

        [Fact]
        public void Should_specify_that_sealed_types_cannot_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(SealedType), A.Dummy<IEnumerable<Type>>(), A.Dummy<IEnumerable<object>>(), A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("The type of proxy FakeItEasy.Tests.Creation.CastleDynamicProxy.CastleDynamicProxyGeneratorTests+SealedType is sealed.");
        }

        [Fact]
        [UsingCulture("en-US")]
        public void Should_specify_that_no_default_constructor_was_found()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(ClassWithPrivateConstructor), Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ReasonForFailure.Should().StartWith("No usable default constructor was found on the type");
        }

        [Fact]
        [UsingCulture("en-US")]
        public void Should_specify_that_private_class_was_not_found()
        {
            // Arrange

            // Act
            var type = Type.GetType("FluentAssertions.Common.NullReflector, FluentAssertions.Core");
            var result = this.generator.GenerateProxy(type, Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ReasonForFailure.Should().StartWith("No usable default constructor was found on the type FluentAssertions.Common.NullReflector.\r\nAn exception of type Castle.DynamicProxy.Generators.GeneratorException was caught during this call. Its message was:\r\nCan not create proxy for type FluentAssertions.Common.NullReflector because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly FluentAssertions.Core is strong-named.");
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_implement_additional_interfaces(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, new[] { typeof(IFoo) }, null, A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull().And.BeAssignableTo<IFoo>();
        }

        [Fact]
        public void GenerateProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => this.generator.GenerateProxy(typeof(IInterfaceType), Enumerable.Empty<Type>(), null, A.Dummy<IFakeCallProcessorProvider>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Should_pass_arguments_for_constructor_to_constructed_instance()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { 10 },
                A.Dummy<IFakeCallProcessorProvider>());

            var proxy = (TypeWithArgumentsForConstructor)result.GeneratedProxy;

            // Assert
            proxy.Argument.Should().Be(10);
        }

        [Fact]
        public void Should_fail_with_correct_message_when_no_constructor_matches_the_passed_in_arguments()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { "no constructor takes a string" },
                A.Dummy<IFakeCallProcessorProvider>());

            // Assert
            result.ReasonForFailure.Should().StartWith("No constructor matches the passed arguments for constructor.\r\nAn exception of type Castle.DynamicProxy.InvalidProxyConstructorArgumentsException was caught during this call. Its message was:\r\nCan not instantiate proxy of class: FakeItEasy.Tests.Creation.CastleDynamicProxy.CastleDynamicProxyGeneratorTests+TypeWithArgumentsForConstructor.\r\nCould not find a constructor that would match given arguments:\r\nSystem.String\r\n");
        }

        [Fact]
        [UsingCulture("en-US")]
        public void Should_fail_when_arguments_for_constructor_is_passed_with_interface_proxy()
        {
            // Arrange
            var arguments = new object[] { "no constructor on interface " };

            // Act
            var ex = Record.Exception(() => this.generator.GenerateProxy(typeof(IInterfaceType), Enumerable.Empty<Type>(), arguments, A.Dummy<IFakeCallProcessorProvider>()));

            // Assert
            ex.Should().BeAnExceptionOfType<ArgumentException>()
                .WithMessage("Arguments for constructor specified for interface type.");
        }

        [Theory]
        [MemberData(nameof(SupportedTypes))]
        public void Should_be_able_to_intercept_ToString(Type typeOfProxy)
        {
            // Arrange
            var fakeCallProcessorProvider = CreateFakeCallProcessorProvider(c => c.SetReturnValue("interception return value"));

            // Act
            var proxy = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, fakeCallProcessorProvider);
            var toStringResult = proxy.GeneratedProxy.ToString();

            // Assert
            toStringResult.Should().Be("interception return value");
        }

        [Fact]
        public void Should_delegate_to_interception_validator_when_validating_if_method_can_be_intercepted()
        {
            // Arrange
            var method = typeof(object).GetMethod("ToString");
            var instance = new object();

            // Act
            this.generator.MethodCanBeInterceptedOnInstance(method, instance, out Ignore.This<string>().Value);

            // Assert
            A.CallTo(() => this.interceptionValidator
                .MethodCanBeInterceptedOnInstance(method, instance, out Ignore.This<string>().Value)).MustHaveHappened();
        }

        private static IFakeCallProcessorProvider CreateFakeCallProcessorProvider(Action<IInterceptedFakeObjectCall> fakeCallProcessorAction)
        {
            var result = A.Fake<IFakeCallProcessorProvider>();

            var fakeCallProcessor = A.Fake<IFakeCallProcessor>();
            A.CallTo(() => result.Fetch(A<object>._)).Returns(fakeCallProcessor);

            A.CallTo(() => fakeCallProcessor.Process(A<IInterceptedFakeObjectCall>._)).Invokes(fakeCallProcessorAction);

            return result;
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        public class TypeWithArgumentsForConstructor
        {
            public TypeWithArgumentsForConstructor(int argument)
            {
                this.Argument = argument;
            }

            public int Argument { get; set; }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        public abstract class AbstractClass
            : IInterfaceType
        {
            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        public class ClassWithProtectedConstructor
            : IInterfaceType
        {
            protected ClassWithProtectedConstructor()
            {
            }

            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        public class ClassWithInternalConstructor
            : IInterfaceType
        {
            internal ClassWithInternalConstructor()
            {
            }

            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        public class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        internal class InternalType
            : IInterfaceType
        {
            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

        private sealed class SealedType
        {
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class SerializableFakeCallProcessorProvider : IFakeCallProcessorProvider
        {
            public IFakeCallProcessor Fetch(object proxy)
            {
                throw new NotSupportedException();
            }

            public void EnsureInitialized(object proxy)
            {
            }

            public void EnsureManagerIsRegistered()
            {
            }
        }
    }
}
