namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using Xunit;

    public class CastleDynamicProxyGeneratorTests
    {
        private readonly ReadOnlyCollection<Type> noAdditionalInterfaces = new ReadOnlyCollection<Type>(new List<Type>());

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
        public interface IInterfaceType
        {
        }

        [Theory]
        [InlineData(typeof(IInterfaceType))]
        [InlineData(typeof(AbstractClass))]
        [InlineData(typeof(ClassWithProtectedConstructor))]
        [InlineData(typeof(ClassWithInternalConstructorVisibleToDynamicProxy))]
        [InlineData(typeof(InternalClassVisibleToDynamicProxy))]
        public void Should_ensure_fake_call_processor_is_initialized_but_not_fetched_when_no_method_on_fake_is_called(Type typeThatImplementsInterfaceType)
        {
            // Arrange
            var fakeCallProcessorProvider = A.Fake<IFakeCallProcessorProvider>();

            // Act
            CastleDynamicProxyGenerator.GenerateProxy(typeThatImplementsInterfaceType, this.noAdditionalInterfaces, null, Enumerable.Empty<Expression<Func<Attribute>>>(), fakeCallProcessorProvider);

            // Assert
            A.CallTo(() => fakeCallProcessorProvider.Fetch(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => fakeCallProcessorProvider.EnsureInitialized(A<object>._)).MustHaveHappened();
        }

        [Fact]
        public void GenerateProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => CastleDynamicProxyGenerator.GenerateProxy(typeof(IInterfaceType), this.noAdditionalInterfaces, null, Enumerable.Empty<Expression<Func<Attribute>>>(), A.Dummy<IFakeCallProcessorProvider>());
            call.Should().BeNullGuarded();
        }

        public abstract class AbstractClass
        {
            public virtual void Foo(int argument1, int argument2)
            {
            }
        }

        public class ClassWithProtectedConstructor
        {
            protected ClassWithProtectedConstructor()
            {
            }
        }
    }
}
