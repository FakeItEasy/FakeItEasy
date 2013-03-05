namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using NUnit.Framework;

    [TestFixture]
    public class CastleDynamicProxyGeneratorTests
    {
        private CastleDynamicProxyGenerator generator;
        private CastleDynamicProxyInterceptionValidator interceptionValidator;

        private object[] supportedTypes = new object[] 
        {
            typeof(IInterfaceType),
            typeof(AbstractClass),
            typeof(ClassWithProtectedConstructor),
            typeof(ClassWithInternalConstructor),
            typeof(InternalType)
        };

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
        public void SetUp()
        {
            this.interceptionValidator = A.Fake<CastleDynamicProxyInterceptionValidator>();

            this.generator = new CastleDynamicProxyGenerator(this.interceptionValidator);
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_that_implements_ITaggable(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.GeneratedProxy, Is.InstanceOf<ITaggable>());
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_where_tag_can_be_set(Type typeOfProxy)
        {
            // Arrange
            var tag = new object();

            // Act
            var proxy = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null).GeneratedProxy as ITaggable;
            proxy.Tag = tag;

            // Assert
            Assert.That(proxy.Tag, Is.SameAs(tag));
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_proxy_that_is_of_the_specified_type(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.GeneratedProxy, Is.InstanceOf(typeOfProxy));
        }

        [TestCaseSource("supportedTypes")]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_true(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.True);
        }

        [TestCaseSource("notSupportedTypes")]
        public void Should_return_result_with_ProxyWasSuccessfullyGenerated_set_to_false_when_proxy_can_not_be_generated(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.ProxyWasSuccessfullyGenerated, Is.False);
        }

        [TestCaseSource("supportedTypes")]
        public void Should_raise_event_on_event_raiser_when_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            var result = this.generator.GenerateProxy(typeThatImplementsInterfaceType, Enumerable.Empty<Type>(), null);
            IWritableFakeObjectCall callMadeToProxy = null;

            result.CallInterceptedEventRaiser.CallWasIntercepted += (sender, e) =>
                {
                    callMadeToProxy = e.Call;
                };

            var proxy = (IInterfaceType)result.GeneratedProxy;

            // Act
            proxy.Foo(1, 2);

            // Assert
            Assert.That(callMadeToProxy, Is.Not.Null);
            Assert.That(callMadeToProxy.Arguments, Is.EquivalentTo(new object[] { 1, 2 }));
            Assert.That(callMadeToProxy.Method.Name, Is.EqualTo(typeof(IInterfaceType).GetMethod("Foo").Name));
            Assert.That(callMadeToProxy.FakedObject, Is.SameAs(proxy));
        }

        [TestCaseSource("supportedTypes")]
        public void Generated_proxies_should_be_serializable(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, new Type[] { }, null);

            // Assert
            Assert.That(result.GeneratedProxy, Is.BinarySerializable);
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_specify_that_value_types_can_not_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(int), Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.ReasonForFailure, Is.EqualTo("The type of proxy must be an interface or a class but it was System.Int32."));
        }

        [Test]
        public void Should_specify_that_sealed_types_can_not_be_generated()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(SealedType), A.Dummy<IEnumerable<Type>>(), A.Dummy<IEnumerable<object>>());

            // Assert
            Assert.That(result.ReasonForFailure, Is.EqualTo("The type of proxy \"FakeItEasy.Tests.Creation.CastleDynamicProxy.CastleDynamicProxyGeneratorTests+SealedType\" is sealed."));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_specify_that_no_default_constructor_was_found()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(ClassWithPrivateConstructor), Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.ReasonForFailure, Is.StringStarting("No default constructor was found on the type"));
        }

        [TestCaseSource("supportedTypes")]
        public void Should_implement_additional_interfaces(Type typeOfProxy)
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeOfProxy, new[] { typeof(IFoo) }, null);

            // Assert
            Assert.That(result.GeneratedProxy, Is.InstanceOf<IFoo>());
        }

        [Test]
        public void TryCreateProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.generator.GenerateProxy(typeof(IInterfaceType), Enumerable.Empty<Type>(), null));
        }

        [Test]
        public void Should_pass_arguments_for_constructor_to_constructed_instance()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { 10 });

            var proxy = (TypeWithArgumentsForConstructor)result.GeneratedProxy;

            // Assert
            Assert.That(proxy.Argument, Is.EqualTo(10));
        }

        [Test]
        public void Should_fail_with_correct_message_when_no_constructor_matches_the_passed_in_arguments()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(
                typeof(TypeWithArgumentsForConstructor),
                Enumerable.Empty<Type>(),
                new object[] { "no constructor takes a string" });

            // Assert
            Assert.That(result.ReasonForFailure, Is.EqualTo("No constructor matches the passed arguments for constructor."));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_fail_when_arguments_for_constructor_is_passed_with_interface_proxy()
        {
            // Arrange

            // Act, Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var result = this.generator.GenerateProxy(
                        typeof(IInterfaceType),
                        Enumerable.Empty<Type>(),
                        new object[] { "no constructor on interface " });
                });

            Assert.That(ex.Message, Is.EqualTo("Arguments for constructor specified for interface type."));
        }

        [TestCaseSource("supportedTypes")]
        public void Should_be_able_to_intercept_ToString(Type typeOfProxy)
        {
            // Arrange
            bool wasCalled = false;

            // Act
            var proxy = this.generator.GenerateProxy(typeOfProxy, Enumerable.Empty<Type>(), null);

            proxy.CallInterceptedEventRaiser.CallWasIntercepted += (s, e) =>
                {
                    wasCalled = true;
                };

            proxy.GeneratedProxy.ToString();

            // Assert
            Assert.That(wasCalled, Is.True);
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
        public class TypeWithNoneOfTheObjectMethodsOverridden
        {
        }

        [Serializable]
        public class TypeWithAllOfTheObjectMethodsOverridden
        {
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        [Serializable]
        public class TypeWithNonVirtualProperty
        {
            public int Foo { get; set; }
        }

        [Serializable]
        public class TypeWithInternalInterceptableProperties
        {
            internal virtual string ReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            internal virtual string WriteOnly
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            internal virtual string Normal
            {
                get;
                set;
            }

            internal virtual string ReadOnlyAutomatic
            {
                get;
                private set;
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
