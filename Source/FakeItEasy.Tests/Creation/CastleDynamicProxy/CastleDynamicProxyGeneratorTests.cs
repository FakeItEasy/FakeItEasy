namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class CastleDynamicProxyGeneratorTests
    {
        private CastleDynamicProxyGenerator generator;
        private CastleDynamicProxyInterceptionValidator interceptionValidator;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] supportedTypes = new object[] 
        {
            typeof(IInterfaceType),
            typeof(AbstractClass),
            typeof(ClassWithProtectedConstructor),
            typeof(ClassWithInternalConstructor),
            typeof(InternalType)
        };

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] notSupportedTypes = new object[] 
        {
            typeof(int),
            typeof(ClassWithPrivateConstructor)
        };

        public interface IInterfaceType
        {
            void Foo(int argument1, int argument2);
        }

        [SetUp]
        public void Setup()
        {
            this.interceptionValidator = A.Fake<CastleDynamicProxyInterceptionValidator>();

            this.generator = new CastleDynamicProxyGenerator(this.interceptionValidator);
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_that_can_be_tagged(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull().And.BeAssignableTo<ITaggable>();
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_where_tag_can_be_set(Type typeOfProxy)
        {
            // Arrange
            var tag = new object();

            // Act
            var proxy = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>()).GeneratedProxy as ITaggable;
            proxy.Tag = tag;

            // Assert
            proxy.Tag.Should().BeSameAs(tag);
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_that_is_of_the_specified_type(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull()
                .And.Subject.Should().Match(p => typeOfProxy.IsAssignableFrom(p.GetType()));
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_true(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeTrue();
        }

        [TestCaseSource("notSupportedTypes")]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_false_when_proxy_cannot_be_generated(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ProxyWasSuccessfullyGenerated.Should().BeFalse();
        }

        [TestCaseSource("supportedTypes")]
        public void Should_delegate_to_interception_sink_when_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            IWritableFakeObjectCall interceptedFakeObjectCall = null;

            var lazyInterceptionSinkProvider = CreateInterceptionSinkProvider(c => interceptedFakeObjectCall = c);

            var result = this.generator.GenerateProxy(typeThatImplementsInterfaceType, Enumerable.Empty<Type>(), null, lazyInterceptionSinkProvider);

            var proxy = (IInterfaceType)result.GeneratedProxy;

            // Act
            proxy.Foo(1, 2);

            // Assert
            interceptedFakeObjectCall.Should().NotBeNull();
            interceptedFakeObjectCall.Arguments.Should().BeEquivalentTo(1, 2);
            interceptedFakeObjectCall.Method.Name.Should().Be(typeof(IInterfaceType).GetMethod("Foo").Name);
            interceptedFakeObjectCall.FakedObject.Should().BeSameAs(proxy);
        }

        [TestCaseSource("supportedTypes")]
        public void Should_ensure_interception_sink_is_initialized_but_not_fetched_when_no_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            var lazyInterceptionSinkProvider = A.Fake<ILazyInterceptionSinkProvider>();

            // Act
            this.generator.GenerateProxy(typeThatImplementsInterfaceType, Enumerable.Empty<Type>(), null, lazyInterceptionSinkProvider);

            // Assert
            A.CallTo(() => lazyInterceptionSinkProvider.Fetch(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => lazyInterceptionSinkProvider.EnsureInitialized(A<object>._)).MustHaveHappened();
        }

        [TestCaseSource("supportedTypes")]
        public void Serialized_proxies_should_deserialize_to_an_object(Type typeOfProxy)
        {
            // Arrange
            var result = this.generator.GenerateProxy(typeOfProxy, new Type[] { }, null, A.Dummy<ILazyInterceptionSinkProvider>());
            var proxy = result.GeneratedProxy;
            var serializer = new BinaryFormatter();
            using (var stream = new System.IO.MemoryStream())
            {
                // Act
                serializer.Serialize(stream, proxy);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                var deserializedProxy = serializer.Deserialize(stream);

                // Assert
                deserializedProxy.Should().NotBeNull();
            }
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_specify_that_value_types_cannot_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(int), Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("The type of proxy must be an interface or a class but it was System.Int32.");
        }

        [Test]
        public void Should_specify_that_sealed_types_cannot_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(SealedType), A.Dummy<IEnumerable<Type>>(), A.Dummy<IEnumerable<object>>(), A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("The type of proxy \"FakeItEasy.Tests.Creation.CastleDynamicProxy.CastleDynamicProxyGeneratorTests+SealedType\" is sealed.");
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_specify_that_no_default_constructor_was_found()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(ClassWithPrivateConstructor), Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ReasonForFailure.Should().StartWith("No usable default constructor was found on the type");
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_specify_that_private_class_was_not_found()
        {
            // Arrange

            // Act
            var type = Type.GetType("System.AppDomainInitializerInfo, mscorlib");
            var result = this.generator.GenerateProxy(type, Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("No usable default constructor was found on the type System.AppDomainInitializerInfo.\r\nAn exception was caught during this call. Its message was:\r\nCan not create proxy for type System.AppDomainInitializerInfo because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly mscorlib is strong-named.");
        }

        [TestCaseSource("supportedTypes")]
        public void Should_implement_additional_interfaces(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, new[] { typeof(IFoo) }, null, A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.GeneratedProxy.Should().NotBeNull().And.BeAssignableTo<IFoo>();
        }

        [Test]
        public void GenerateProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.generator.GenerateProxy(typeof(IInterfaceType), Enumerable.Empty<Type>(), null, A.Dummy<ILazyInterceptionSinkProvider>()));
        }

        [Test]
        public void Should_pass_arguments_for_constructor_to_constructed_instance()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { 10 },
                A.Dummy<ILazyInterceptionSinkProvider>());

            var proxy = (TypeWithArgumentsForConstructor)result.GeneratedProxy;

            // Assert
            proxy.Argument.Should().Be(10);
        }

        [Test]
        public void Should_fail_with_correct_message_when_no_constructor_matches_the_passed_in_arguments()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { "no constructor takes a string" },
                A.Dummy<ILazyInterceptionSinkProvider>());

            // Assert
            result.ReasonForFailure.Should().Be("No constructor matches the passed arguments for constructor.\r\nAn exception was caught during this call. Its message was:\r\nCan not instantiate proxy of class: FakeItEasy.Tests.Creation.CastleDynamicProxy.CastleDynamicProxyGeneratorTests+TypeWithArgumentsForConstructor.\r\nCould not find a constructor that would match given arguments:\r\nSystem.String\r\n");
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_fail_when_arguments_for_constructor_is_passed_with_interface_proxy()
        {
            // Arrange
            var arguments = new object[] { "no constructor on interface " };

            // Act
            var ex = Record.Exception(() => this.generator.GenerateProxy(typeof(IInterfaceType), Enumerable.Empty<Type>(), arguments, A.Dummy<ILazyInterceptionSinkProvider>()));

            // Assert
            ex.Should().BeAnExceptionOfType<ArgumentException>()
                .WithMessage("Arguments for constructor specified for interface type.");
        }

        [TestCaseSource("supportedTypes")]
        public void Should_be_able_to_intercept_ToString(Type typeOfProxy)
        {
            // Arrange
            var lazyInterceptionSinkProvider = CreateInterceptionSinkProvider(c => c.SetReturnValue("interception return value"));

            // Act
            var proxy = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null, lazyInterceptionSinkProvider);
            var toStringResult = proxy.GeneratedProxy.ToString();

            // Assert
            toStringResult.Should().Be("interception return value");
        }

        [Test]
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

        private static ILazyInterceptionSinkProvider CreateInterceptionSinkProvider(Action<IWritableFakeObjectCall> actionToInvoke)
        {
            var lazyInterceptionSinkProvider = A.Fake<ILazyInterceptionSinkProvider>();

            var interceptionSink = A.Fake<IInterceptionSink>();
            A.CallTo(() => lazyInterceptionSinkProvider.Fetch(A<object>._)).Returns(interceptionSink);

            A.CallTo(() => interceptionSink.Process(A<IWritableFakeObjectCall>._)).Invokes(actionToInvoke);

            return lazyInterceptionSinkProvider;
        }

        [Serializable]
        public class TypeWithArgumentsForConstructor
        {
            public TypeWithArgumentsForConstructor(int argument)
            {
                this.Argument = argument;
            }

            public int Argument { get; set; }
        }

        [Serializable]
        public abstract class AbstractClass
            : IInterfaceType
        {
            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

        [Serializable]
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

        [Serializable]
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

        [Serializable]
        public class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }

        [Serializable]
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
    }
}
