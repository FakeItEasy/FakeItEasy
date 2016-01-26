namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class CreationSpecs
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface ICollectionItem
        {
        }

        [Scenario]
        public static void ThrowingConstructor(
            Exception exception)
        {
            "When faking a class whose constructor throws"
                .x(() => exception = Record.Exception(() => A.Fake<ClassWhoseConstructorThrows>()));

            "It should throw a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message should include the original exception type"
                .x(() => exception.Message.Should().Contain("of type System.NotSupportedException"));

            "And the exception message should include the original exception message"
                .x(() => exception.Message.Should().Contain("I don't like being constructed."));

            "And the exception message should include the original exception stack trace"
                .x(() => exception.Message.Should().Contain("FakeItEasy.Specs.CreationSpecs.ClassWhoseConstructorThrows..ctor()"));
        }

        // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
        [Scenario]
        public static void UseSuccessfulConstructor(
            FakedClass fake)
        {
            "When faking a class whose first constructor fails"
                .x(() => fake = A.Fake<FakedClass>());

            "It should instantiate the fake using the successful constructor with the longest parameter list"
                .x(() => fake.WasTwoParameterConstructorCalled.Should().BeTrue());

            "And the fake should not remember the failing constructor call"
                .x(() => fake.WasParameterlessConstructorCalled
                             .Should().BeFalse("because the parameterless constructor was called for a different fake object"));

            "And it should only have tried the parameterless constructor and one with the longest parameter list"
                .x(() => FakedClass.ParameterListLengthsForAttemptedConstructors.Should().BeEquivalentTo(0, 2));
        }

        [Scenario]
        [Example(2)]
        [Example(10)]
        public static void CollectionOfFake(
            int count,
            IList<ICollectionItem> fakes)
        {
            "When creating a collection of {0} fakes"
                .x(() => fakes = A.CollectionOfFake<ICollectionItem>(count));

            "Then {0} items should be created"
                .x(() => fakes.Should().HaveCount(count));

            "And all items should extend the specified type"
                .x(() => fakes.Should().ContainItemsAssignableTo<ICollectionItem>());

            "And all items should be fakes"
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) != null));
        }

        public class ClassWhoseConstructorThrows
        {
            public ClassWhoseConstructorThrows()
            {
                throw new NotSupportedException("I don't like being constructed.");
            }
        }

        public class FakedClass
        {
            private static ISet<int> parameterListLengthsForAttemptedConstructors = new SortedSet<int>();

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                parameterListLengthsForAttemptedConstructors.Add(0);
                this.WasParameterlessConstructorCalled = true;

                throw new InvalidOperationException();
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface)
            {
                parameterListLengthsForAttemptedConstructors.Add(1);
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someName", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface, string someName)
            {
                parameterListLengthsForAttemptedConstructors.Add(2);
                this.WasTwoParameterConstructorCalled = true;
            }

            public static ISet<int> ParameterListLengthsForAttemptedConstructors
            {
                get { return parameterListLengthsForAttemptedConstructors; }
            }

            public virtual bool WasParameterlessConstructorCalled { get; set; }

            public virtual bool WasTwoParameterConstructorCalled { get; set; }
        }
    }
}