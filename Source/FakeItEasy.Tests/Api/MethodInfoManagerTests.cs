namespace FakeItEasy.Tests.Api
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using FakeItEasy.Core;

    [TestFixture]
    public class MethodInfoManagerTests
    {
        private MethodInfoManager CreateManager()
        {
            return new MethodInfoManager();
        }

        private MethodInfo GetMethod<T>(Expression<Action<T>> methodSpecification)
        {
            return ((MethodCallExpression)methodSpecification.Body).Method;
        }

        private MethodInfo GetMethod<T, TReturn>(Expression<Func<T, TReturn>> methodSpecification)
        {
            return ((MethodCallExpression)methodSpecification.Body).Method;
        }

        [Test]
        public void Matches_returns_false_when_methods_are_not_the_same()
        {
            var first = this.GetMethod<BaseType>(x => x.DoSomething());
            var second = this.GetMethod<BaseType>(x => x.ToString());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(BaseType), first, second), Is.False);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_true_when_both_methods_are_same()
        {
            var first = this.GetMethod<BaseType>(x => x.DoSomething());
            var second = this.GetMethod<BaseType>(x => x.DoSomething());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(BaseType), first, second), Is.True);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_true_when_first_is_method_on_sub_type()
        {
            var first = this.GetMethod<SubType>(x => x.DoSomething());
            var second = this.GetMethod<BaseType>(x => x.DoSomething());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(SubType), first, second), Is.True);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_true_when_first_is_method_on_base_type()
        {
            var first = this.GetMethod<BaseType>(x => x.DoSomething());
            var second = this.GetMethod<SubType>(x => x.DoSomething());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(BaseType), first, second), Is.True);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_false_when_calls_are_to_same_method_but_with_different_generic_arguments()
        {
            var first = this.GetMethod<IHaveAGenericMethod>(x => x.GenericMethod<string>());
            var second = this.GetMethod<IHaveAGenericMethod>(x => x.GenericMethod<int>());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second), Is.False);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_true_when_first_points_to_interface_method_and_second_to_implementing_method()
        {
            var first = this.GetMethod<IInterface>(x => x.Foo());
            var second = this.GetMethod<InterfaceImplementor>(x => x.Foo());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(InterfaceImplementor), first, second), Is.True);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_true_when_one_points_to_interface_method_with_generic_argument_and_second_points_to_implementor()
        {
            var first = this.GetMethod<IHaveAGenericMethod>(x => x.GenericMethod<int>());
            var second = this.GetMethod<HaveAGenericMethod>(x => x.GenericMethod<int>());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second), Is.True);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_false_when_one_points_to_interface_method_with_generic_argument_and_second_points_to_implementor_but_with_different_type_argument()
        {
            var first = this.GetMethod<IHaveAGenericMethod>(x => x.GenericMethod<int>());
            var second = this.GetMethod<HaveAGenericMethod>(x => x.GenericMethod<string>());

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(HaveAGenericMethod), first, second), Is.False);
        }

        [Test]
        public void WillInvokeSameMethodOnTarget_should_return_false_when_calls_are_to_same_method_but_to_different_overloads()
        {
            var first = this.GetMethod<BaseType>(x => x.DoSomething());
            var second = this.GetMethod<BaseType>(x => x.DoSomething("a"));

            var manager = this.CreateManager();

            Assert.That(manager.WillInvokeSameMethodOnTarget(typeof(BaseType), first, second), Is.False);
        }

        public interface IInterface
        {
            void Foo();
        }

        public class InterfaceImplementor
            : IInterface
        {
            public void Foo()
            {
                throw new NotImplementedException();
            }
        }


        public interface IHaveAGenericMethod
        {
            void GenericMethod<T>();
        }

        public class HaveAGenericMethod
            : IHaveAGenericMethod
        {
            public void GenericMethod<T>()
            {
                throw new NotImplementedException();
            }
        }


        public class BaseType
            : IEquatable<BaseType>
        {
            public virtual void DoSomething()
            {

            }

            public void DoSomething(string a)
            {

            }

            public T GetDefault<T>()
            {
                return default(T);
            }

            public bool Equals(BaseType other)
            {
                return true;
            }
        }

        public class SubType
            : BaseType
        {
            public override void DoSomething()
            {

            }
        }
    }
}
