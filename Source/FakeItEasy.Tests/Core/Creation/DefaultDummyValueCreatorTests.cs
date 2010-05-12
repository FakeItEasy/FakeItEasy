namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultDummyValueCreatorTests
    {
        private IProxyGenerator proxyGenerator;
        private DelegateFakeObjectContainer container;
        private IConstructorResolver constructorResolver;
        private DefaultDummyValueCreator dummyCreator;
        private IFakeCreationSession session;

        [SetUp]
        public void SetUp()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.container = new DelegateFakeObjectContainer();
            this.constructorResolver = A.Fake<IConstructorResolver>();

            this.session = A.Fake<IFakeCreationSession>();

            A.CallTo(() => this.session.ProxyGenerator).Returns(this.proxyGenerator);
            A.CallTo(() => this.session.ConstructorResolver).Returns(this.constructorResolver);            

            this.dummyCreator = new DefaultDummyValueCreator(this.session, this.container);
        }

        [Test]
        public void Should_returned_value_cached_in_session_when_available()
        {
            // Arrange
            object outValue = null;
            A.CallTo(() => this.session.TryGetCachedDummyValue(typeof(string), out outValue))
                .Returns(true).AssignsOutAndRefParameters("in session");

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(string), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo("in session"));
        }

        [Test]
        public void Should_resolve_interface_dummy_by_calling_proxy_generator()
        {
            // Arrange
            var resultFromProxyGenerator = A.Fake<IFoo>() as IFakedProxy;

            var proxyResult = A.Fake<ProxyResult>();
            A.CallTo(() => proxyResult.Proxy).Returns(resultFromProxyGenerator);
            A.CallTo(() => proxyResult.ProxyWasSuccessfullyCreated).Returns(true);
            
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<IEnumerable<Type>>.That.Matches(x => x.Count() == 0).Argument, A<FakeManager>.That.Not.IsNull(), null))
                .Returns(proxyResult);

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(IFoo), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.SameAs(resultFromProxyGenerator));
        }

        [Test]
        public void Should_resolve_value_type_by_returning_default_value()
        {
            // Arrange
            
            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(int), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo(0));
        }

        [Test]
        public void Should_resolve_from_container_when_available()
        {
            // Arrange
            this.container.Register<string>(() => "registered in container");

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(string), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo("registered in container"));
        }

        [Test]
        public void Should_insert_resolved_value_in_session()
        {
            // Arrange

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(int), out dummy);

            // Assert
            A.CallTo(() => this.session.AddResolvedDummyValueToCache(typeof(int), 0)).MustHaveHappened();
        }

        [Test]
        public void Should_insert_type_as_being_resolved_in_session()
        {
            // Arrange

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(DateTime), out dummy);

            // Assert
            A.CallTo(() => this.session.RegisterTriedToResolveType(typeof(DateTime))).MustHaveHappened();
        }

        [Test]
        public void Should_fail_to_resolve_when_session_reports_that_type_has_failed_earlier()
        {
            // Arrange
            A.CallTo(() => this.session.TypeHasFailedToResolve(typeof(int))).Returns(true);

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(int), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_fail_to_resolve_when_type_is_delegate_type()
        {
            // Arrange
            A.CallTo(() => this.constructorResolver.ListAllConstructors(typeof(Func<int>)))
                .Returns(new[] 
                {
                    new ConstructorAndArgumentsInfo(
                        typeof(TypeWithConstructors).GetConstructor(new[] { typeof(object), typeof(IntPtr) }),
                        new[] 
                        { 
                            new ArgumentInfo(true, typeof(object), new object()),
                            new ArgumentInfo(true, typeof(IntPtr), IntPtr.Zero)
                        }
                    )
                });

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(Func<int>), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_resolve_by_activating_when_constructor_is_resolved()
        {
            // Arrange
            A.CallTo(() => this.constructorResolver.ListAllConstructors(typeof(TypeWithConstructors)))
                .Returns(new[] 
                {
                    new ConstructorAndArgumentsInfo(
                        typeof(TypeWithConstructors).GetConstructor(new[] { typeof(IFoo), typeof(string) }),
                        new[] 
                        { 
                            new ArgumentInfo(true, typeof(IFoo), A.Fake<IFoo>()),
                            new ArgumentInfo(true, typeof(string), "foo")
                        }
                    )
                });

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(TypeWithConstructors), out dummy);

            // Assert
            var constructed = dummy as TypeWithConstructors;
            Assert.That(constructed.Argument, Is.InstanceOf<IFoo>());
            Assert.That(constructed.Argument2, Is.EqualTo("foo"));
        }

        [Test]
        public void Should_fail_gracefully_when_activated_constructor_throws()
        {
            // Arrange
            A.CallTo(() => this.constructorResolver.ListAllConstructors(typeof(TypeWithConstructorThatThrows)))
                .Returns(new[] 
                { 
                    new ConstructorAndArgumentsInfo(
                        typeof(TypeWithConstructorThatThrows).GetConstructor(new Type[] { }),
                        Enumerable.Empty<ArgumentInfo>()
                    )
                });

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(TypeWithConstructorThatThrows), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_fail_gracefully_when_type_is_void()
        {
            // Arrange

            // Act
            object dummy = null;
            var result = this.dummyCreator.TryCreateDummyValue(typeof(void), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        public class TypeWithConstructors
        {
            public IFoo Argument;
            public string Argument2;

            public TypeWithConstructors(IFoo argument, string argument2)
            {
                this.Argument = argument;
                this.Argument2 = argument2;
            }
        }

        public class TypeWithConstructorThatThrows
        {
            public TypeWithConstructorThatThrows()
            {
                throw new Exception();
            }
        }
    }
}
