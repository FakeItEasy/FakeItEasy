namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Core;
    using ExtensionSyntax;
    using NUnit.Framework;
    using Tests;

    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Faked_object_configured_to_call_back_performs_callback_when_called()
        {
            var foo = A.Fake<IFoo>();

            bool wasCalled = false;

            foo.Configure().CallsTo(x => x.Bar()).Invokes(x => wasCalled = true);
            
            foo.Bar();

            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_perform_several_call_backs_and_return_value_does_all()
        {
            var foo = A.Fake<IFoo>();
            
            bool firstWasCalled = false;
            bool secondWasCalled = false;

            foo.Configure()
                .CallsTo(x => x.Baz())
                .Invokes(x => firstWasCalled = true)
                .Invokes(x => secondWasCalled = true)
                .Returns(10);

            var result = foo.Baz();

            Assert.That(firstWasCalled);
            Assert.That(secondWasCalled);
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_call_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.False);

            fake.Configure().CallsTo(x => x.DoSomething()).CallsBaseMethod();
            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_return_value_from_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.Configure().CallsTo(x => x.ReturnSomething()).CallsBaseMethod();
            
            Assert.That(fake.ReturnSomething(), Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_invokes_configured_invokations_also()
        {
            var fake = A.Fake<BaseClass>();

            bool wasCalled = false;

            fake.Configure().CallsTo(x => x.ReturnSomething()).Invokes(x => wasCalled = true).CallsBaseMethod();
            
            fake.ReturnSomething();

            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void Function_call_can_be_configured_using_predicate_to_validate_arguments()
        {
            var fake = A.Fake<IFoo>();

            fake.Configure()
                .CallsTo(x => x.Baz(null, null))
                .WhenArgumentsMatch(x => true)
                .Returns(100);

            Assert.That(fake.Baz("something", "else"), Is.EqualTo(100));
        }

        [Test]
        public void Void_call_can_be_configured_using_predicate_to_validate_arguments()
        {
            var fake = A.Fake<IFoo>();

            fake.Configure()
                .CallsTo(x => x.Bar(null, null))
                .WhenArgumentsMatch(x => true)
                .Throws(new Exception());            
            
            Assert.Throws<Exception>(() =>
                fake.Bar("something", "else"));
        }

        [Test]
        public void Output_and_reference_parameters_can_be_configured()
        {
            var fake = A.Fake<IDictionary<string, string>>();
            string bla = null;
            
            fake.Configure()
                .CallsTo(x => x.TryGetValue("test", out bla))
                .Returns(true)
                .AssignsOutAndRefParameters("bla");

            fake.TryGetValue("test", out bla);

            Assert.That(bla, Is.EqualTo("bla"));
        }

        [Test]
        public void Void_methods_can_be_configured_by_the_all_in_one_syntax()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            
            // Act
            A.CallTo(() => foo.Bar(A<string>._, A<string>._)).Throws(new FormatException());
            
            // Assert
            Assert.Throws<FormatException>(() =>
                foo.Bar("any", "thing"));
        }

        [Test]
        public void Functions_can_be_configured_by_the_all_in_one_syntax()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            
            // Act
            A.CallTo(() => foo.Baz(null, null)).WithAnyArguments().Returns(99);
            
            // Assert
            Assert.That(foo.Baz("any", "thing"), Is.EqualTo(99));
        }

        [Test]
        public void Should_be_able_to_configure_any_call_with_a_specific_return_type_to_return_value()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            Any.CallTo(foo).WithReturnType<int>().Returns(10);
            Any.CallTo(foo).WithReturnType<string>().Returns("foo");
            
            // Assert
            Assert.That(foo.SomeProperty, Is.EqualTo(10));
            Assert.That(foo.Baz(), Is.EqualTo(10));
        }

        [Test]
        public void A_fake_should_be_passed_to_the_container_to_be_configured_when_created()
        {
            // Arrange
            var container = A.Fake<IFakeObjectContainer>();
            Any.CallTo(container).WithReturnType<bool>().Returns(false);

            using (Fake.CreateScope(container))
            {
                // Act
                var fake = A.Fake<IFoo>();

                // Assert
                A.CallTo(() => container.ConfigureFake(typeof(IFoo), fake)).MustHaveHappened();
            }
        }

        [Test]
        public void Should_be_able_to_specify_predicates_when_configuring_any_call_on_an_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(foo).Where(x => x.Method.Name.Equals("Bar")).Throws(new Exception());
            A.CallTo(() => foo.Bar()).Throws(new Exception());
            
            // Assert
            Assert.DoesNotThrow(() => foo.Baz());
            Assert.Throws<Exception>(foo.Bar);
        }

        [Test]
        public void Should_be_able_to_configure_indexed_properties()
        {
            // Arrange
            var fake = A.Fake<IIndexed>(x => x.Strict());

            A.CallTo(() => fake[10]).Returns("ten");
            
            // Act
            
            // Assert
            Assert.That(fake[10], Is.EqualTo("ten"));
        }

        [Test]
        public void Should_be_able_to_intercept_protected_method()
        {
            // Arrange
            var fake = A.Fake<TypeWithProtectedMethod>();

            // Act
            A.CallTo(fake).WithReturnType<int>().Where(x => x.Method.Name == "ProtectedMethod").Returns(20);

            // Assert
            Assert.That(fake.CallsProtectedMethod(), Is.EqualTo(20));
        }

        public class TypeWithProtectedMethod
        {
            public int CallsProtectedMethod()
            {
                return this.ProtectedMethod();
            }

            protected virtual int ProtectedMethod()
            {
                return 10;
            }
        }

        public interface IIndexed
        {
            string this[int index] { get; }
        }

        public class BaseClass
        {
            public bool WasCalled;

            public virtual void DoSomething()
            {
                WasCalled = true;
            }

            public virtual int ReturnSomething()
            {
                return 10;
            }
        }
    }
}
