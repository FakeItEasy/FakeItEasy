namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xbehave;

    public class ClassWhoseConstructorThrows
    {
        public ClassWhoseConstructorThrows()
        {
            throw new NotSupportedException("I don't like being constructed.");
        }
    }

    public class Creation
    {
        [Scenario]
        public void when_faking_a_class_whose_constructor_throws(
            Exception exception)
        {
            "when faking a class whose constructor throws"
                .x(() => exception = Catch.Exception(() => A.Fake<ClassWhoseConstructorThrows>()));

            "it should throw a FakeCreationException"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "it should throw an exception whose message includes original exception type"
                .x(() => exception.Message.Should().Contain("of type System.NotSupportedException"));

            "it should throw an exception whose message includes original exception message"
                .x(() => exception.Message.Should().Contain("I don't like being constructed."));

            "it should throw an exception whose message includes original exception stack trace"
                .x(() => exception.Message.Should().Contain("FakeItEasy.Specs.ClassWhoseConstructorThrows..ctor()"));
        }

        // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
        [Scenario]
        public void when_faking_a_class_whose_first_constructor_fails(
            FakedClass fake)
        {
            "when faking a class whose constructor throws"
                .x(() => fake = A.Fake<FakedClass>());

            "it should instantiate the fake using the successful constructor with the longest parameter list"
                .x(() => fake.WasTwoParameterConstructorCalled.Should().BeTrue());

            "it should instantiate a fake that does not remember the failing constructor call"
                .x(() => fake.WasParameterlessConstructorCalled
                             .Should().BeFalse("because the parameterless constructor was called for a different fake object"));

            "it should only have tried the parameterless constructor and one with the longest parameter list"
                .x(() => FakedClass.ParameterListLengthsForAttemptedConstructors.Should().BeEquivalentTo(0, 2));
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