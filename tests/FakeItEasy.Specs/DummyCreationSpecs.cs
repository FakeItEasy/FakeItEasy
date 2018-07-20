namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public abstract class DummyCreationSpecsBase
    {
        [Scenario]
        public void InterfaceCreation(
            IDisposable dummy)
        {
            "Given a fakeable interface type"
                .See<IDisposable>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<IDisposable>());

            "Then it returns a fake of that type"
                .x(() => dummy.Should().BeAFake());
        }

        [Scenario]
        public void AbstractClassCreation(
            TextReader dummy)
        {
            "Given a fakeable abstract class type"
                .See<TextReader>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<TextReader>());

            "Then it returns a fake of that type"
                .x(() => dummy.Should().BeAFake());
        }

        [Scenario]
        public void ValueTypeCreation(
            DateTime dummy)
        {
            "Given a value type"
                .See<DateTime>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<DateTime>());

            "Then it returns the default value for that type"
                .x(() => dummy.Should().Be(default(DateTime)));
        }

        [Scenario]
        public void NullableTypeCreation(
            int? dummy)
        {
            "Given a nullable type"
                .See<int?>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<int?>());

            "Then it returns the default value for that type"
                .x(() => dummy.Should().Be(default(int?)));
        }

        [Scenario]
        public void TypeWithDummyFactoryCreation(
            Foo dummy)
        {
            "Given a type"
                .See<Foo>();

            "And a dummy factory for that type"
                .See<FooDummyFactory>();

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<Foo>());

            "Then it returns a dummy created by the dummy factory"
                .x(() => dummy.Bar.Should().Be(42));
        }

        [Scenario]
        public void CollectionOfDummyCreation(
            IList<DateTime> dummies)
        {
            "Given a type"
                .See<DateTime>();

            "When a collection of that type is requested"
                .x(() => dummies = this.CreateCollectionOfDummy<DateTime>(10));

            "Then it returns a collection with the specified number of dummies"
                .x(() => dummies.Should().HaveCount(10));
        }

        [Scenario]
        public void ClassWhoseLongerConstructorThrowsCreation(
            ClassWhoseLongerConstructorThrows dummy1, ClassWhoseLongerConstructorThrows dummy2)
        {
            "Given a type with multiple constructors"
                .See<ClassWhoseLongerConstructorThrows>();

            "And its longer constructor throws"
                .See(() => new ClassWhoseLongerConstructorThrows(0, 0));

            "And a dummy of that type is requested"
                .x(() => dummy1 = this.CreateDummy<ClassWhoseLongerConstructorThrows>());

            "And another dummy of that type is requested"
                .x(() => dummy2 = this.CreateDummy<ClassWhoseLongerConstructorThrows>());

            "Then it returns a dummy from the first request"
                .x(() => dummy1.Should().NotBeNull());

            "And the dummy is created via the shorter constructor"
                .x(() => dummy1.CalledConstructor.Should().Be("(int)"));

            "And it returns a dummy from the second request"
                .x(() => dummy2.Should().NotBeNull());

            "And that dummy is created via the shorter constructor"
                .x(() => dummy2.CalledConstructor.Should().Be("(int)"));

            "And the dummies are distinct"
                .x(() => dummy1.Should().NotBeSameAs(dummy2));

            "And the longer constructor was only attempted once"
                .x(() => ClassWhoseLongerConstructorThrows.NumberOfTimesLongerConstructorWasCalled.Should().Be(1));
        }

        [Scenario]
        public void SealedClassWhoseLongerConstructorThrowsCreation(
            SealedClassWhoseLongerConstructorThrows dummy)
        {
            "Given a sealed type with multiple constructors"
                .See<SealedClassWhoseLongerConstructorThrows>();

            "And its longer constructor throws"
                .See(() => new SealedClassWhoseLongerConstructorThrows(0, 0));

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<SealedClassWhoseLongerConstructorThrows>());

            "Then it returns a dummy"
                .x(() => dummy.Should().NotBeNull());

            "And the dummy is created via the shorter constructor"
                .x(() => dummy.CalledConstructor.Should().Be("(int)"));
        }

        [Scenario]
        public void ClassWithLongConstructorWhoseArgumentsCannotBeResolvedCreation(
            ClassWithLongConstructorWhoseArgumentsCannotBeResolved dummy)
        {
            "Given a type with multiple constructors"
                .See<ClassWithLongConstructorWhoseArgumentsCannotBeResolved>();

            "And its longer constructor's argument cannot be resolved"
                .See(() => new ClassWithLongConstructorWhoseArgumentsCannotBeResolved(default(ClassWhoseDummyFactoryThrows), default(int)));

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<ClassWithLongConstructorWhoseArgumentsCannotBeResolved>());

            "Then it returns a dummy"
                .x(() => dummy.Should().NotBeNull());

            "And the dummy is created via the shorter constructor"
                .x(() => dummy.CalledConstructor.Should().Be("(int)"));
        }

        [Scenario]
        public void SealedClassWithLongConstructorWhoseArgumentsCannotBeResolvedCreation(
            SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved dummy)
        {
            "Given a sealed type with multiple constructors"
                .See<SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved>();

            "And its longer constructor's argument cannot be resolved"
                .See(() => new SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved(default(ClassWhoseDummyFactoryThrows), default(int)));

            "When a dummy of that type is requested"
                .x(() => dummy = this.CreateDummy<SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved>());

            "Then it returns a dummy"
                .x(() => dummy.Should().NotBeNull());

            "And the dummy is created via the shorter constructor"
                .x(() => dummy.CalledConstructor.Should().Be("(int)"));
        }

        protected abstract T CreateDummy<T>();

        protected abstract IList<T> CreateCollectionOfDummy<T>(int count);

        public class Foo
        {
            public int Bar { get; set; }
        }

        public class FooDummyFactory : DummyFactory<Foo>
        {
            protected override Foo Create()
            {
                return new Foo { Bar = 42 };
            }
        }

        public sealed class ClassWhoseLongerConstructorThrows
        {
            private static int numberOfTimesLongerConstructorWasCalled;

            public string CalledConstructor { get; }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public ClassWhoseLongerConstructorThrows(int i)
            {
                this.CalledConstructor = "(int)";
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "j", Justification = "Required for testing.")]
            public ClassWhoseLongerConstructorThrows(int i, int j)
            {
                this.CalledConstructor = "(int, int)";
                Interlocked.Increment(ref numberOfTimesLongerConstructorWasCalled);
                throw new Exception("(int, int) constructor threw");
            }

            public static int NumberOfTimesLongerConstructorWasCalled => numberOfTimesLongerConstructorWasCalled;
        }

        public sealed class SealedClassWhoseLongerConstructorThrows
        {
            public string CalledConstructor { get; }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public SealedClassWhoseLongerConstructorThrows(int i)
            {
                this.CalledConstructor = "(int)";
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "j", Justification = "Required for testing.")]
            public SealedClassWhoseLongerConstructorThrows(int i, int j)
            {
                this.CalledConstructor = "(int, int)";
                throw new Exception("(int, int) constructor threw");
            }
        }

        public class ClassWithLongConstructorWhoseArgumentsCannotBeResolved
        {
            public string CalledConstructor { get; }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public ClassWithLongConstructorWhoseArgumentsCannotBeResolved(int i)
            {
                this.CalledConstructor = "(int)";
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "c", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public ClassWithLongConstructorWhoseArgumentsCannotBeResolved(ClassWhoseDummyFactoryThrows c, int i)
            {
                this.CalledConstructor = "(ClassWhoseDummyFactoryThrows, int)";
            }
        }

        public sealed class SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved
        {
            public string CalledConstructor { get; }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved(int i)
            {
                this.CalledConstructor = "(int)";
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "c", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "i", Justification = "Required for testing.")]
            public SealedClassWithLongConstructorWhoseArgumentsCannotBeResolved(ClassWhoseDummyFactoryThrows c, int i)
            {
                this.CalledConstructor = "(ClassWhoseDummyFactoryThrows, int)";
            }
        }

        public class ClassWhoseDummyFactoryThrows
        {
        }

        public class ClassWhoseDummyFactoryThrowsFactory : DummyFactory<ClassWhoseDummyFactoryThrows>
        {
            protected override ClassWhoseDummyFactoryThrows Create()
            {
                throw new Exception("dummy factory threw");
            }
        }
    }

    public class GenericDummyCreationSpecs : DummyCreationSpecsBase
    {
        protected override T CreateDummy<T>()
        {
            return A.Dummy<T>();
        }

        protected override IList<T> CreateCollectionOfDummy<T>(int count)
        {
            return A.CollectionOfDummy<T>(count);
        }
    }

    public class NonGenericDummyCreationSpecs : DummyCreationSpecsBase
    {
        protected override T CreateDummy<T>()
        {
            return (T)Sdk.Create.Dummy(typeof(T));
        }

        protected override IList<T> CreateCollectionOfDummy<T>(int count)
        {
            return Sdk.Create.CollectionOfDummy(typeof(T), count).Cast<T>().ToList();
        }
    }
}
