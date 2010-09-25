namespace FakeItEasy.Tests.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.DynamicProxy;
    using NUnit.Framework;

    [TestFixture]
    public class CastleDynamicProxyGeneratorNewTests
    {
        private CastleDynamicProxyGenerator generator;

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

        private MemberInfo[] nonInterceptableMembers = new MemberInfo[] 
            {
                typeof(object).GetMethod("GetType"),
                typeof(string).GetProperty("Length"),
                typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetGetMethod(),
                typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetSetMethod(),
                typeof(TypeWithNonVirtualProperty).GetProperty("Foo")
            };

        private MemberInfo[] interceptableMembers = new MemberInfo[] 
            {
                typeof(TypeWithNoneOfTheObjectMethodsOverridden).GetMethod("ToString", new Type[] {}),
                typeof(TypeWithNoneOfTheObjectMethodsOverridden).GetMethod("GetHashCode", new Type[] {}),
                typeof(TypeWithNoneOfTheObjectMethodsOverridden).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(object).GetMethod("GetHashCode", new Type[] {}),
                typeof(object).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(IFoo).GetMethod("Bar", new Type[] {}),
                typeof(IFoo).GetProperty("SomeProperty").GetGetMethod(),
                typeof(object).GetMethod("ToString", new Type[] {}),
                typeof(IFoo).GetProperty("SomeProperty").GetSetMethod(),
                typeof(IFoo).GetProperty("SomeProperty"),
                typeof(TypeWithAllOfTheObjectMethodsOverridden).GetMethod("ToString", new Type[] {}),
                typeof(TypeWithAllOfTheObjectMethodsOverridden).GetMethod("GetHashCode", new Type[] {}),
                typeof(TypeWithAllOfTheObjectMethodsOverridden).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(TypeWithInternalInterceptableProperties).GetProperty("WriteOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(TypeWithInternalInterceptableProperties).GetProperty("Normal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnlyAutomatic", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            };

        [SetUp]
        public void SetUp()
        {
            this.generator = new CastleDynamicProxyGenerator();
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
        [SetCulture("en-US")]
        public void Should_specify_that_no_default_constructor_was_found()
        {
            // Arrange

            // Act
            var result = this.generator.GenerateProxy(typeof(ClassWithPrivateConstructor), Enumerable.Empty<Type>(), null);

            // Assert
            Assert.That(result.ReasonForFailure, Text.StartsWith("No default constructor was found on the type"));
        }

        [TestCaseSource("nonInterceptableMembers")]
        public void MemberCanBeIntercepted_should_return_false_for_non_virtual_member(MemberInfo member)
        {
            // Arrange
            
            // Act
            var result = this.generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCaseSource("interceptableMembers")]
        public void MemberCanBeIntercepted_should_return_true_for_virtual_member(MemberInfo member)
        {
            // Arrange
            
            // Act
            var result = this.generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.True, "Was not able to intercept the member");
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
        public void MemberCanBeIntercepted_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() => 
                this.generator.MemberCanBeIntercepted(typeof(object).GetMethod("GetHashCode")));
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

        [Serializable]
        public class TypeWithArgumentsForConstructor
        {
            public int Argument;

            public TypeWithArgumentsForConstructor(int argument)
            {
                this.Argument = argument;
            }
        }
       
        public interface IInterfaceType
        {
            void Foo(int argument1, int argument2);
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

        [Serializable]
        public class TypeWithNoneOfTheObjectMethodsOverridden
        { }
        
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
    }
}
