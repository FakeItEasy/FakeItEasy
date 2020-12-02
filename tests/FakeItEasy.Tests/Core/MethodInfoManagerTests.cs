namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    using static FakeItEasy.Tests.TestHelpers.ExpressionHelper;

    public class MethodInfoManagerTests
    {
        public interface IInterface
        {
            void Foo();
        }

        public interface IHaveAGenericMethod
        {
            [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Required for testing.")]
            void GenericMethod<T>();
        }

        [Fact]
        public void Matches_returns_false_when_methods_are_not_the_same()
        {
            var first = GetMethodInfo<Base>(x => x.DoSomething());
            var second = GetMethodInfo<Base>(x => x.ToString());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Base), first, second).Should().BeFalse();
        }

        [Fact]
        public void Will_invoke_same_method_on_target_when_both_methods_are_same()
        {
            var first = GetMethodInfo<Base>(x => x.DoSomething());
            var second = GetMethodInfo<Base>(x => x.DoSomething());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Base), first, second).Should().BeTrue();
        }

        [Fact]
        public void Will_invoke_same_method_on_target_when_first_is_method_on_derived_type()
        {
            var first = GetMethodInfo<Derived>(x => x.DoSomething());
            var second = GetMethodInfo<Base>(x => x.DoSomething());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Derived), first, second).Should().BeTrue();
        }

        [Fact]
        public void Will_invoke_same_method_on_target_when_first_is_method_on_base_type()
        {
            var first = GetMethodInfo<Base>(x => x.DoSomething());
            var second = GetMethodInfo<Derived>(x => x.DoSomething());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Base), first, second).Should().BeTrue();
        }

        [Fact]
        public void Will_not_invoke_same_method_on_target_when_calls_are_to_same_method_but_with_different_generic_arguments()
        {
            var first = GetMethodInfo<IHaveAGenericMethod>(x => x.GenericMethod<string>());
            var second = GetMethodInfo<IHaveAGenericMethod>(x => x.GenericMethod<int>());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second).Should().BeFalse();
        }

        [Fact]
        public void Will_invoke_same_method_on_target_when_first_points_to_interface_method_and_second_to_implementing_method()
        {
            var first = GetMethodInfo<IInterface>(x => x.Foo());
            var second = GetMethodInfo<Concrete>(x => x.Foo());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Concrete), first, second).Should().BeTrue();
        }

        [Fact]
        public void Will_invoke_same_method_on_target_when_one_points_to_interface_method_with_generic_argument_and_second_to_implementing_method()
        {
            var first = GetMethodInfo<IHaveAGenericMethod>(x => x.GenericMethod<int>());
            var second = GetMethodInfo<HaveAGenericMethod>(x => x.GenericMethod<int>());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second).Should().BeTrue();
        }

        [Fact]
        public void Will_not_invoke_same_method_on_target_when_one_points_to_interface_method_with_generic_argument_and_second_to_implementing_method_but_with_different_type_argument()
        {
            var first = GetMethodInfo<IHaveAGenericMethod>(x => x.GenericMethod<int>());
            var second = GetMethodInfo<HaveAGenericMethod>(x => x.GenericMethod<string>());

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second).Should().BeFalse();
        }

        [Fact]
        public void Will_not_invoke_same_method_on_target_when_calls_are_to_same_method_but_to_different_overloads()
        {
            var first = GetMethodInfo<Base>(x => x.DoSomething());
            var second = GetMethodInfo<Base>(x => x.DoSomething("a"));

            var manager = this.CreateManager();

            manager.WillInvokeSameMethodOnTarget(typeof(Base), first, second).Should().BeFalse();
        }

        [Fact]
        public void Should_return_true_when_method_is_explicit_implementation_of_interface_method()
        {
            // Arrange
            var explicitImplementation = typeof(ConcreteWithExplicitImplementation).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == "FakeItEasy.Tests.Core.MethodInfoManagerTests.IInterface.Foo");
            var interfaceMethod = GetMethodInfo<IInterface>(x => x.Foo());

            var manager = this.CreateManager();

            // Act

            // Assert
            manager.WillInvokeSameMethodOnTarget(typeof(ConcreteWithExplicitImplementation), explicitImplementation, interfaceMethod).Should().BeTrue();
        }

        [Fact]
        public void Should_return_false_when_same_method_is_invoked_on_generic_type_with_different_type_arguments()
        {
            // Arrange
            var first = GetMethodInfo<GenericType<int>>(x => x.Foo());
            var second = GetMethodInfo<GenericType<string>>(x => x.Foo());
            var manager = this.CreateManager();

            // Act

            // Assert
            manager.WillInvokeSameMethodOnTarget(typeof(GenericType<int>), first, second).Should().BeFalse();
        }

        [Fact]
        public void Should_return_method_from_derived_type_when_getting_non_virtual_method_on_base_type()
        {
            // Arrange
            var method = GetMethodInfo<Derived>(x => x.DoSomething("foo"));

            var manager = this.CreateManager();

            // Act, Assert
            manager.GetMethodOnTypeThatWillBeInvokedByMethodInfo(typeof(Derived), method)
                .Should().NotBeNull().And.Subject
                .Name.Should().Be(nameof(Derived.DoSomething));
        }

        private MethodInfoManager CreateManager()
        {
            return new MethodInfoManager();
        }

        public class ConcreteWithExplicitImplementation : IInterface
        {
            public void Foo()
            {
            }

            void IInterface.Foo()
            {
                throw new NotImplementedException();
            }
        }

        public class Concrete : IInterface
        {
            public void Foo()
            {
                throw new NotImplementedException();
            }
        }

        public class HaveAGenericMethod : IHaveAGenericMethod
        {
            public void GenericMethod<T>()
            {
                throw new NotImplementedException();
            }
        }

        public class Base : IEquatable<Base>
        {
            public virtual void DoSomething()
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "text", Justification = "Required for testing.")]
            public void DoSomething(string text)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "other", Justification = "Required for testing.")]
            public bool Equals([AllowNull] Base other) => true;

            public override bool Equals(object? obj) => true;

            public override int GetHashCode() => 0;
        }

        public class Derived : Base
        {
            public override void DoSomething()
            {
            }
        }

        public class GenericType<T>
        {
            public void Foo()
            {
            }
        }
    }
}
