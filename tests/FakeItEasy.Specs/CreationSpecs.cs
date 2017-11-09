namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
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
                .See(() => new FakedClass(A.Dummy<IDisposable>()));

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
                .x(() => parameterListLengthsForAttemptedConstructors.Should().BeEquivalentTo(0, 2));
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
            public FakedClass(IDisposable someInterface)
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
