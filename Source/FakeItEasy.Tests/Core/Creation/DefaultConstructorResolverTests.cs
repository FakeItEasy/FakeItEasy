namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultConstructorResolverTests
    {
        //[Test]
        //public void GenerateProxy_without_arguments_for_constructor_should_return_true_when_container_can_resolve_arguments_for_constructor()
        //{
        //    A.CallTo(() => this.container.TryCreateFakeObject(typeof(string), out Null<object>.Out)).Returns(true).AssignsOutAndRefParameters("foo");

        //    var generator = this.CreateGenerator();

        //    var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), null, this.fakeObject, null);

        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        //}

        //[Test]
        //public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_value_type()
        //{
        //    var generator = this.CreateGenerator();

        //    var result = generator.GenerateProxy(typeof(TypeThatTakesValueTypeInConstructor), null, this.fakeObject, null);

        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        //}

        //[Test]
        //public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_interface_type()
        //{
        //    var generator = this.CreateGenerator();

        //    var result = generator.GenerateProxy(typeof(TypeThatTakesProxyableTypeInConstructor), null, this.fakeObject, null);

        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        //}

        //[Test]
        //public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_class_with_default_constructor()
        //{
        //    var generator = this.CreateGenerator();

        //    var result = generator.GenerateProxy(typeof(SealedTypeThatTakesClassWithDefaultConstructorInConstructor), null, this.fakeObject, null);

        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        //}


        //[Test]
        //public void GenerateProxy_should_return_false_when_resolvable_constructors_contains_circular_dependencies()
        //{
        //    var generator = this.CreateGenerator();
        //    var result = generator.GenerateProxy(typeof(CircularClassA), null, this.fakeObject, null);

        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        //}

        //[Test]
        //public void Should_return_result_with_false_when_type_depends_on_delegate()
        //{
        //    // Arrange
        //    var generator = this.CreateGenerator();

        //    // Act
        //    var result = generator.GenerateProxy(typeof(TypeThatDependsOnDelegate), Enumerable.Empty<Type>(), this.fakeObject, null);

        //    // Assert
        //    Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        //}

        public class CircularClassA
        {
            public CircularClassA(CircularClassB b)
            { }
        }

        public class CircularClassB
        {
            public CircularClassB(CircularClassA a)
            {

            }
        }

        public class TypeThatTakesProxyableTypeInConstructor
        {
            public TypeThatTakesProxyableTypeInConstructor(IFormattable formattable)
            {

            }
        }

        public class TypeThatTakesValueTypeInConstructor
        {
            public TypeThatTakesValueTypeInConstructor(int value)
            {

            }
        }

        public class TypeWithNoResolvableConstructor
        {
            public TypeWithNoResolvableConstructor(IFoo foo, NoInstanceType bar)
            {

            }

            protected TypeWithNoResolvableConstructor(NoInstanceType bar)
            {

            }

            internal TypeWithNoResolvableConstructor(NoInstanceType bar, string foo)
            {

            }

            private TypeWithNoResolvableConstructor()
            {

            }

            public class NoInstanceType
            {
                private NoInstanceType()
                {

                }
            }
        }

        public class TypeWithAbstractArgumentToConstructor
        {
            public TypeWithAbstractArgumentToConstructor(AbstractClassWithDefaultConstructor argument)
            {

            }
        }

        public abstract class AbstractClassWithDefaultConstructor
        {

        }


        //public class SealedTypeThatTakesClassWithDefaultConstructorInConstructor
        //{
        //    public SealedTypeThatTakesClassWithDefaultConstructorInConstructor(TypeWithDefaultConstructor a)
        //    {

        //    }
        //}
    }
}
