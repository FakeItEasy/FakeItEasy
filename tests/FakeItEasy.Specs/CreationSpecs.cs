namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public abstract class CreationSpecsBase
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface ICollectionItem
        {
        }

        public interface IInterfaceWithSimilarMethods
        {
            void Test1<T>(IEnumerable<T> enumerable);

            void Test1<T>(IList<T> enumerable);
        }

        [Scenario]
        public void ThrowingConstructor(
            Exception exception)
        {
            "Given a class with a parameterless constructor"
                .See<ClassWhoseConstructorThrows>();

            "And the constructor throws an exception"
                .See(() => new ClassWhoseConstructorThrows());

            "When I create a fake of the class"
                .x(() => exception = Record.Exception(() => this.CreateFake<ClassWhoseConstructorThrows>()));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message includes the original exception type"
                .x(() => exception.Message.Should().Contain("of type System.NotSupportedException"));

            "And the exception message includes the original exception message"
                .x(() => exception.Message.Should().Contain("I don't like being constructed."));

            "And the exception message includes the original exception stack trace"
                .x(() => exception.Message.Should().Contain("FakeItEasy.Specs.CreationSpecsBase.ClassWhoseConstructorThrows..ctor()"));
        }

        // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
        [Scenario]
        public void UseSuccessfulConstructor(
            FakedClass fake,
            IEnumerable<int> parameterListLengthsForAttemptedConstructors)
        {
            "Given a class with multiple constructors"
                .See<FakedClass>();

            "And the parameterless constructor throws"
                .See(() => new FakedClass());

            "And the class has a one-parameter constructor"
                .See(() => new FakedClass(A.Dummy<ArgumentThatShouldNeverBeResolved>()));

            "And the class has a two-parameter constructor"
                .See(() => new FakedClass(A.Dummy<IDisposable>(), A.Dummy<string>()));

            "When I create a fake of the class"
                .x(() =>
                {
                    lock (FakedClass.ParameterListLengthsForAttemptedConstructors)
                    {
                        FakedClass.ParameterListLengthsForAttemptedConstructors.Clear();
                        fake = this.CreateFake<FakedClass>();
                        parameterListLengthsForAttemptedConstructors = new List<int>(FakedClass.ParameterListLengthsForAttemptedConstructors);
                    }
                });

            "Then the fake is instantiated using the two-parameter constructor"
                .x(() => fake.WasTwoParameterConstructorCalled.Should().BeTrue());

            "And the fake doesn't remember the failing constructor call"
                .x(() => fake.WasParameterlessConstructorCalled
                             .Should().BeFalse("because the parameterless constructor was called for a different fake object"));

            "And the one-parameter constructor was not tried"
                .x(() => parameterListLengthsForAttemptedConstructors.Should().NotContain(1));

            "And the argument for the unused constructor was never resolved"
                .x(() => ArgumentThatShouldNeverBeResolved.WasResolved.Should().BeFalse());
        }

        [Scenario]
        public void CacheSuccessfulConstructor(
            ClassWhosePreferredConstructorsThrow fake1,
            ClassWhosePreferredConstructorsThrow fake2)
        {
            "Given a class with multiple constructors"
                .See<ClassWhosePreferredConstructorsThrow>();

            "And the class has a parameterless constructor that throws"
                .See(() => new ClassWhosePreferredConstructorsThrow());

            "And the class has a two-parameter constructor that throws"
                .See(() => new ClassWhosePreferredConstructorsThrow(A.Dummy<IDisposable>(), A.Dummy<string>()));

            "And the class has a one-parameter constructor that succeeds"
                .See(() => new ClassWhosePreferredConstructorsThrow(A.Dummy<int>()));

            "When I create a fake of the class"
                .x(() => fake1 = this.CreateFake<ClassWhosePreferredConstructorsThrow>());

            "And I create another fake of the class"
                .x(() => fake2 = this.CreateFake<ClassWhosePreferredConstructorsThrow>());

            "Then the two fakes are distinct"
                .x(() => fake1.Should().NotBeSameAs(fake2));

            "And the parameterless constructor was only called once"
                .x(() => ClassWhosePreferredConstructorsThrow.NumberOfTimesParameterlessConstructorWasCalled.Should().Be(1));

            "And the two-parameter constructor was only called once"
                .x(() => ClassWhosePreferredConstructorsThrow.NumberOfTimesTwoParameterConstructorWasCalled.Should().Be(1));
        }

        public class ClassWhosePreferredConstructorsThrow
        {
            public static int NumberOfTimesParameterlessConstructorWasCalled => numberOfTimesParameterlessConstructorWasCalled;

            public static int NumberOfTimesTwoParameterConstructorWasCalled => numberOfTimesTwoParameterConstructorWasCalled;

            private static int numberOfTimesTwoParameterConstructorWasCalled;

            private static int numberOfTimesParameterlessConstructorWasCalled;

            public ClassWhosePreferredConstructorsThrow()
            {
                Interlocked.Increment(ref numberOfTimesParameterlessConstructorWasCalled);
                throw new NotImplementedException();
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "anInt", Justification = "This is just a dummy argument.")]
            public ClassWhosePreferredConstructorsThrow(int anInt)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposable", Justification = "This is just a dummy argument.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "aString", Justification = "This is just a dummy argument.")]
            public ClassWhosePreferredConstructorsThrow(IDisposable disposable, string aString)
            {
                Interlocked.Increment(ref numberOfTimesTwoParameterConstructorWasCalled);
                throw new NotImplementedException();
            }
        }

        [Scenario]
        [Example(2)]
        [Example(10)]
        public void CollectionOfFake(
            int count,
            IList<ICollectionItem> fakes)
        {
            "When I create a collection of {0} fakes"
                .x(() => fakes = this.CreateCollectionOfFake<ICollectionItem>(count));

            "Then {0} items are created"
                .x(() => fakes.Should().HaveCount(count));

            "And all items extend the specified type"
                .x(() => fakes.Should().ContainItemsAssignableTo<ICollectionItem>());

            "And all items are fakes"
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) != null));
        }

        [Scenario]
        [Example(2)]
        [Example(10)]
        public void CollectionOfFakeWithOptionBuilder(
            int count,
            IList<ICollectionItem> fakes)
        {
            "When I create a collection of {0} fakes that also implement another interface"
                .x(() => fakes = this.CreateCollectionOfFake<ICollectionItem>(count, options => options.Implements<IDisposable>()));

            "Then {0} items are created"
                .x(() => fakes.Should().HaveCount(count));

            "And all items extend the specified type and the extra interface"
                .x(() => fakes.Should().ContainItemsAssignableTo<ICollectionItem>().And.ContainItemsAssignableTo<IDisposable>());

            "And all items are fakes"
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) != null));
        }

        [Scenario]
        public void InterfaceWithAlikeGenericMethod(IInterfaceWithSimilarMethods fake)
        {
            "Given an interface with an overloaded methods containing generic arguments"
                .See<IInterfaceWithSimilarMethods>();

            "When I create a fake of the interface"
                .x(() => fake = this.CreateFake<IInterfaceWithSimilarMethods>());

            "Then the fake is created"
                .x(() => fake.Should().BeAFake());
        }

        [Scenario]
        public void SealedClassCannotBeFaked(Exception exception)
        {
            "Given a sealed class"
                .See<SealedClass>();

            "When I create a fake of the class"
                .x(() => exception = Record.Exception(() => this.CreateFake<SealedClass>()));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().Be(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+SealedClass.
    The type of proxy FakeItEasy.Specs.CreationSpecsBase+SealedClass is sealed.
"));
        }

        [Scenario]
        public void StructCannotBeFaked(Exception exception)
        {
            "Given a struct"
                .See<Struct>();

            "When I create a fake of the struct"
                .x(() => exception = Record.Exception(() => this.CreateFake<Struct>()));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().Be(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+Struct.
    The type of proxy must be an interface or a class but it was FakeItEasy.Specs.CreationSpecsBase+Struct.
"));
        }

        protected abstract T CreateFake<T>();

        protected abstract T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder);

        protected abstract IList<T> CreateCollectionOfFake<T>(int numberOfFakes);

        protected abstract IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder);

        public class ClassWhoseConstructorThrows
        {
            public ClassWhoseConstructorThrows()
            {
                throw new NotSupportedException("I don't like being constructed.");
            }
        }

        public class FakedClass
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                ParameterListLengthsForAttemptedConstructors.Add(0);
                this.WasParameterlessConstructorCalled = true;

                throw new InvalidOperationException();
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            public FakedClass(ArgumentThatShouldNeverBeResolved argument)
            {
                ParameterListLengthsForAttemptedConstructors.Add(1);
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someName", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface, string someName)
            {
                ParameterListLengthsForAttemptedConstructors.Add(2);
                this.WasTwoParameterConstructorCalled = true;
            }

            public static ISet<int> ParameterListLengthsForAttemptedConstructors { get; } = new SortedSet<int>();

            public bool WasParameterlessConstructorCalled { get; set; }

            public bool WasTwoParameterConstructorCalled { get; set; }
        }

        public sealed class ArgumentThatShouldNeverBeResolved
        {
            public static bool WasResolved { get; private set; }

            public ArgumentThatShouldNeverBeResolved()
            {
                WasResolved = true;
            }
        }

        public sealed class SealedClass
        {
        }

        public struct Struct
        {
        }
    }

    public class GenericCreationSpecs : CreationSpecsBase
    {
        protected override T CreateFake<T>()
        {
            return A.Fake<T>();
        }

        protected override T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            return A.Fake(optionsBuilder);
        }

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes)
        {
            return A.CollectionOfFake<T>(numberOfFakes);
        }

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder)
        {
            return A.CollectionOfFake(numberOfFakes, optionsBuilder);
        }
    }

    public class NonGenericCreationSpecs : CreationSpecsBase
    {
        protected override T CreateFake<T>()
        {
            return (T)Sdk.Create.Fake(typeof(T));
        }

        protected override T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            return (T)Sdk.Create.Fake(typeof(T), options => optionsBuilder((IFakeOptions<T>)options));
        }

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes)
        {
            return Sdk.Create.CollectionOfFake(typeof(T), numberOfFakes).Cast<T>().ToList();
        }

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder)
        {
            return Sdk.Create.CollectionOfFake(typeof(T), numberOfFakes, options => optionsBuilder((IFakeOptions<T>)options)).Cast<T>().ToList();
        }
    }
}
