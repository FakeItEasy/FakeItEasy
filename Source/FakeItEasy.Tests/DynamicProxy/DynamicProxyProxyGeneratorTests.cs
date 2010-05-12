namespace FakeItEasy.Tests.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.DynamicProxy;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicProxyProxyGeneratorTests
    {
        private FakeManager fakeManager;
        private IConstructorResolver constructorResolver;
        private IFakeCreationSession session;

        [SetUp]
        public void SetUp()
        {
            this.fakeManager = A.Fake<FakeManager>();
            this.session = A.Fake<IFakeCreationSession>();
            this.constructorResolver = A.Fake<IConstructorResolver>(x => x.Wrapping(new ConstructorResolverThatGetsDefaultConstructors()));

            A.CallTo(() => this.session.ConstructorResolver).Returns(this.constructorResolver);
        }

        private class ConstructorResolverThatGetsDefaultConstructors
            : IConstructorResolver
        {
            public IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors(Type type)
            {
                return
                    from constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    where constructor.GetParameters().Length == 0
                    select new ConstructorAndArgumentsInfo
                        {
                            Arguments = Enumerable.Empty<ArgumentInfo>(),
                            Constructor = constructor
                        };
            }
        }

        private DynamicProxyProxyGenerator CreateGenerator()
        {
            return new DynamicProxyProxyGenerator(this.session);
        }

        private List<Type> typesThatCanBeProxied = new List<Type>()
		        {
		            typeof(IFoo),
		            typeof(ClassWithDefaultConstructor),
		            typeof(AbstractClassWithDefaultConstructor)
		        };

        private List<Type> typesThatCanNotBeProxied = new List<Type>()
		        {
		            typeof(int),
		            typeof(AbstractClassWithHiddenConstructor),
		            typeof(ClassWithHiddenConstructor),
		            typeof(SealedClass)
		        };

        private MemberInfo[] nonInterceptableMembers = new MemberInfo[] 
		        {
		            typeof(string).GetMethod("GetType"),
		            typeof(string).GetProperty("Length"),
		            typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetGetMethod(),
		            typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetSetMethod(),
		            typeof(TypeWithNonVirtualProperty).GetProperty("Foo")
		        };

        private MemberInfo[] interceptableMembers = new MemberInfo[] 
		        {
		            typeof(IFoo).GetMethod("Bar", new Type[] {}),
		            typeof(IFoo).GetProperty("SomeProperty").GetGetMethod(),
		            typeof(IFoo).GetProperty("SomeProperty").GetSetMethod(),
		            typeof(IFoo).GetProperty("SomeProperty"),
		            typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
		            typeof(TypeWithInternalInterceptableProperties).GetProperty("WriteOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
		            typeof(TypeWithInternalInterceptableProperties).GetProperty("Normal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
		            typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnlyAutomatic", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
		        };

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

        [TestCaseSource("nonInterceptableMembers")]
        public void MemberCanBeIntercepted_should_return_false_for_non_virtual_member(MemberInfo member)
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCaseSource("interceptableMembers")]
        public void MemberCanBeIntercepted_should_return_true_for_virtual_member(MemberInfo member)
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.True, "Was not able to intercept the member");
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_return_true_when_type_can_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, null, this.fakeManager, null);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_assign_result_to_out_parameter(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, null, this.fakeManager, null);

            Assert.That(result.Proxy, Is.InstanceOf(typeOfProxy));
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_return_proxy_that_access_the_fake_object(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, null, this.fakeManager, null);

            Assert.That(result.Proxy.FakeManager, Is.SameAs(this.fakeManager));

        }

        [TestCaseSource("typesThatCanNotBeProxied")]
        public void GenerateProxy_should_return_false_for_types_that_can_not_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, null, this.fakeManager, null);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        }

        [Test]
        public void GenerateProxy_should_throw_when_type_is_interface_but_arguments_for_constructor_is_specified()
        {
            var generator = this.CreateGenerator();

            var thrown = Assert.Throws<ArgumentException>(() =>
                generator.GenerateProxy(typeof(IFoo), null, this.fakeManager, new object[] { 1, 2 }));

            Assert.That(thrown.Message, Text.StartsWith("Arguments for constructor was specified when generating proxy of interface type."));
        }

        [TestCase(typeof(ISomeInterface))]
        [TestCase(typeof(SomeInterfaceImplementation))]
        [TestCase(typeof(SomeAbstractInterfaceImplementation))]
        public void GenerateProxy_should_generate_result_that_raises_events_when_calls_are_intercepted(Type someInterfaceType)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(someInterfaceType, null, this.fakeManager, null);

            IWritableFakeObjectCall interceptedCall = null;
            result.CallWasIntercepted += (s, e) =>
            {
                interceptedCall = e.Call;
            };

            var someInterface = (ISomeInterface)result.Proxy;
            someInterface.Bar();

            Assert.That(interceptedCall.Method.Name, Is.EqualTo("Bar"));
        }

        [Test]
        public void GenerateProxy_should_return_true_when_generating_class_with_arguments_for_constructor_that_matches_constructor()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), null, this.fakeManager, new object[] { "foo" });

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GenerateProxy_with_arguments_for_constructor_should_generate_proxies_raises_events_when_calls_made_to_proxy()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), null, this.fakeManager, new object[] { "foo" });

            IWritableFakeObjectCall interceptedCall = null;
            result.CallWasIntercepted += (s, e) =>
            {
                interceptedCall = e.Call;
            };

            var fake = result.Proxy as TypeWithConstructorThatTakesSingleString;
            fake.Bar();

            Assert.That(interceptedCall.Method.Name, Is.EqualTo("Bar"));
        }

        [Test]
        public void GenerateProxy_with_arguments_for_constructor_should_generate_proxies_that_can_get_proxy_manager()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), null, this.fakeManager, new object[] { "foo" });

            Assert.That(result.Proxy.FakeManager, Is.SameAs(this.fakeManager));
        }

        [Test]
        public void GeneratedProxies_should_be_serializable()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(IFoo), null, this.fakeManager, null);

            Assert.That(result, Is.BinarySerializable);
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_ToString()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(IFoo), null, this.fakeManager, null);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
            };

            result.Proxy.ToString();

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_Equals()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(IFoo), null, this.fakeManager, null);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
                e.Call.SetReturnValue(true);
            };

            result.Proxy.Equals(null);

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_GetHashCode()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(IFoo), null, this.fakeManager, null);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
                e.Call.SetReturnValue(1);
            };

            result.Proxy.GetHashCode();

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GenerateProxy_should_use_widest_resolved_constructor()
        {
            // Arrange
            var generator = this.CreateGenerator();

            A.CallTo(() => this.constructorResolver.ListAllConstructors(typeof(ClassWithDefaultConstructorAndResolvableConstructor)))
                .Returns(new[] 
                {
                    new ConstructorAndArgumentsInfo
                    {
                        Arguments = Enumerable.Empty<ArgumentInfo>(),
                        Constructor = typeof(ClassWithDefaultConstructorAndResolvableConstructor).GetConstructor(new Type[] {})
                    },
                    new ConstructorAndArgumentsInfo
                    {
                        Arguments = new[] 
                        { 
                            new ArgumentInfo(true, typeof(ISomeInterface), A.Fake<ISomeInterface>()),
                            new ArgumentInfo(true, typeof(IFoo), A.Fake<IFoo>())
                        },
                        Constructor  = typeof(ClassWithDefaultConstructorAndResolvableConstructor).GetConstructor(new Type[] { typeof(ISomeInterface), typeof(IFoo) })
                    }
                });

            // Act
            var result = generator.GenerateProxy(typeof(ClassWithDefaultConstructorAndResolvableConstructor), null, this.fakeManager, null);

            // Assert
            var proxy = result.Proxy as ClassWithDefaultConstructorAndResolvableConstructor;
            Assert.That(proxy.WidestConstructorWasCalled, Is.True);
        }

        [Test]
        public void GenerateProxy_should_return_false_when_resolved_constructor_throws_exception()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatThrows), null, this.fakeManager, null);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        }

        [Test]
        public void GenerateProxy_should_return_true_when_type_has_constructor_that_does_not_throw()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow), null, this.fakeManager, null);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_generate_proxy_that_implements_extra_interfaces_when_provided(Type typeOfProxy)
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.GenerateProxy(typeOfProxy, new[] { typeof(IFormatProvider), typeof(IFormattable) }, this.fakeManager, null);

            // Assert
            Assert.That(result.Proxy, Is.InstanceOf<IFormatProvider>());
            Assert.That(result.Proxy, Is.InstanceOf<IFormattable>());
        }

        [Test]
        public void GenerateProxy_should_return_result_that_contains_description_of_non_resolvable_constructors_when_failing()
        {
            // Arrange
            var generator = this.CreateGenerator();

            A.CallTo(() => this.constructorResolver.ListAllConstructors(typeof(TypeWithDifferentConstructors)))
                .Returns(new[] 
                {
                    new ConstructorAndArgumentsInfo
                    {
                        Constructor = GetConstructor<TypeWithDifferentConstructors>(typeof(string)),
                        Arguments = new[]
                        {
                            new ArgumentInfo(false, typeof(string), null)
                        }
                    },
                    new ConstructorAndArgumentsInfo
                    {
                        Constructor = GetConstructor<TypeWithDifferentConstructors>(typeof(int), typeof(object)),
                        Arguments = new[]
                        {
                            new ArgumentInfo(true, typeof(int), 0),
                            new ArgumentInfo(false, typeof(object), null)
                        }
                    },
                    new ConstructorAndArgumentsInfo
                    {
                        Constructor = GetConstructor<TypeWithDifferentConstructors>(typeof(DateTime)),
                        Arguments = new[]
                        {
                            new ArgumentInfo(false, typeof(DateTime), new DateTime())
                        }
                    }
                });

            // Act
            var result = generator.GenerateProxy(typeof(TypeWithDifferentConstructors), Enumerable.Empty<Type>(), this.fakeManager, null);

            // Assert
            Assert.That(result.ErrorMessage, Is.EqualTo(
@"The type has no default constructor and none of the available constructors listed below can be resolved:

public     (*System.String)
internal   (System.Int32, *System.Object)
protected  (*System.DateTime)

* The types marked with with a star (*) can not be faked. Register these types in the current
IFakeObjectContainer in order to generate a fake of this type."));
        }

        private static ConstructorInfo GetConstructor<T>(params Type[] parameterTypes)
        {
            return typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                System.Type.DefaultBinder, parameterTypes, null);
        }

        public class TypeWithDifferentConstructors
        {
            public TypeWithDifferentConstructors(string argument)
            {

            }

            internal TypeWithDifferentConstructors(int argument, object argument2)
            {

            }

            protected TypeWithDifferentConstructors(DateTime argument)
            {

            }
        }

        [Test]
        public void GenerateProxy_should_return_result_that_contains_correct_error_message_when_type_is_sealed()
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.GenerateProxy(typeof(SealedClass), Enumerable.Empty<Type>(), this.fakeManager, null);

            // Assert
            Assert.That(result.ErrorMessage, Is.EqualTo("The type is sealed."));
        }

        public class TypeThatDependsOnDelegate
        {
            public TypeThatDependsOnDelegate(Func<int> d)
            {

            }
        }

        public class TypeWithConstructorThatThrows
        {
            public TypeWithConstructorThatThrows(IServiceProvider a)
            {
                throw new Exception();
            }
        }

        public class TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow
        {
            public TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow(IServiceProvider a)
            {
                throw new Exception();
            }

            public TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow()
            {

            }
        }

        public class ClassWithDefaultConstructorAndResolvableConstructor
        {
            public bool WidestConstructorWasCalled = false;

            public ClassWithDefaultConstructorAndResolvableConstructor()
            { }

            public ClassWithDefaultConstructorAndResolvableConstructor(ISomeInterface a, IFoo b)
            {
                this.WidestConstructorWasCalled = true;
            }
        }

      
        public sealed class TypeWithDefaultConstructor
        {
            public TypeWithDefaultConstructor()
            {

            }
        }

        public interface ISomeInterface
        {
            void Bar();
        }

        public class SomeInterfaceImplementation
            : ISomeInterface
        {

            public virtual void Bar()
            {
                throw new NotImplementedException();
            }
        }

        public abstract class SomeAbstractInterfaceImplementation
            : ISomeInterface
        {

            public abstract void Bar();
        }

        public class TypeWithConstructorThatTakesSingleString
        {
            public TypeWithConstructorThatTakesSingleString(string s)
            { }

            public virtual void Bar()
            {

            }
        }

        public abstract class AbstractClassWithDefaultConstructor
        {

        }
      
        public sealed class SealedClass
        {

        }

        public class ClassWithDefaultConstructor
        {

        }

        public class TypeWithNonVirtualProperty
        {
            public int Foo { get; set; }
        }

        public abstract class AbstractClassWithHiddenConstructor
        {
            private AbstractClassWithHiddenConstructor()
            { }
        }

        public class ClassWithHiddenConstructor
        {
            private ClassWithHiddenConstructor()
            {

            }
        }
    }
}
