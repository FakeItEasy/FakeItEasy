namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

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
        public void StringCreation(
            string dummy1,
            string dummy2)
        {
            "When a dummy string is requested"
                .x(() => dummy1 = this.CreateDummy<string>());

            "And another dummy string is requested"
                .x(() => dummy2 = this.CreateDummy<string>());

            "Then it returns an empty string the first time"
                .x(() => dummy1.Should().Be(string.Empty));

            "Then it returns an empty string the second time"
                .x(() => dummy2.Should().Be(string.Empty));

            "And the two strings are the same reference"
                .x(() => dummy1.Should().BeSameAs(dummy2));
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

            // If multiple theads attempt to create the dummy at the same time, the
            // unsuccessful constructors may be called more than once, so serialize dummy
            // creation for this test.
            "And nobody else is trying to create a dummy of the class right now"
                .x(() => Monitor.TryEnter(typeof(ClassWhoseLongerConstructorThrows), TimeSpan.FromSeconds(30)).Should().BeTrue("we must enter the monitor"))
                .Teardown(() => Monitor.Exit(typeof(ClassWhoseLongerConstructorThrows)));

            "When a dummy of that type is requested"
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

        [Scenario]
        public void ClassWithRecursiveDependencyCreation(Exception exception)
        {
            "Given a type that can't be made into a Dummy because it has a recursive dependency"
                .See<ClassWithRecursiveDependency>();

            "When a dummy of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateDummy<ClassWithRecursiveDependency>()));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created due to the dependency"
                .x(() => exception.Message.Should().Be(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithRecursiveDependency:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.

  The constructors with the following signatures were not tried:
    (*FakeItEasy.Specs.DummyCreationSpecsBase+AnIntermediateClassInTheRecursiveDependencyChain)

    Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

"));
        }

        [Scenario]
        public void ClassWithNoPublicConstructorCreation(Exception exception)
        {
            "Given a type that can't be made into a Dummy because it has no public constructor"
                .See<ClassWithNoPublicConstructors>();

            "When a dummy of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateDummy<ClassWithNoPublicConstructors>()));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created"
                .x(() => exception.Message.Should().StartWith(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.
    It has no public constructors.

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors.
      An exception of type Castle.DynamicProxy.InvalidProxyConstructorArgumentsException was caught during this call. Its message was:
      Can not instantiate proxy of class: FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors.
      Could not find a parameterless constructor.
"));
        }

        [Scenario]
        public void ClassWhoseOnlyConstructorThrowsCreation(Exception exception)
        {
            "Given a type that can't be made into a Dummy because its only public constructor throws"
                .See<ClassWithNoPublicConstructors>();

            "When a dummy of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateDummy<ClassWithThrowingConstructor>()));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created"
                .x(() => exception.Message.Should().StartWith(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithThrowingConstructor:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithThrowingConstructor.
      An exception of type System.Exception was caught during this call. Its message was:
      constructor threw"));
        }

        [Scenario]
        public void PrivateAbstractClassCreation(Exception exception)
        {
            "Given a type that can't be made into a Dummy because is a private abstract class"
                .See<PrivateAbstractClass>();

            "When a dummy of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateDummy<PrivateAbstractClass>()));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created"
                .x(() => exception.Message.Should().Match(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+PrivateAbstractClass:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.
    It is abstract.

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.DummyCreationSpecsBase+PrivateAbstractClass.
      An exception of type Castle.DynamicProxy.Generators.GeneratorException was caught during this call. Its message was:
      Can not create proxy for type FakeItEasy.Specs.DummyCreationSpecsBase+PrivateAbstractClass because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)] attribute*"));
        }

        [Scenario]
        public void ClassWhoseConstructorArgumentsCannotBeResolvedCreation(Exception exception)
        {
            "Given a type that can't be made into a Dummy because the arguments for its constructor cannot be resolved"
                .See<ClassWithNoResolvableConstructors>();

            "When a dummy of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateDummy<ClassWithNoResolvableConstructors>()));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created"
                .x(() => exception.Message.Should().Be(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoResolvableConstructors:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.

  The constructors with the following signatures were not tried:
    (*FakeItEasy.Specs.DummyCreationSpecsBase+ClassWhoseDummyFactoryThrows)

    Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

"));
        }

        [Scenario]
        public void CollectionOfClassWithNoPublicConstructor(Exception exception)
        {
            "Given a type that can't be made into a Dummy"
                .See<ClassWithNoPublicConstructors>();

            "When a collection of dummies of that type is requested"
                .x(() => exception = Record.Exception(() => this.CreateCollectionOfDummy<ClassWithNoPublicConstructors>(1)));

            "Then it throws an exception of type DummyCreationException"
                .x(() => exception.Should().BeAnExceptionOfType<DummyCreationException>());

            "And its message indicates that a dummy couldn't be created"
                .x(() => exception.Message.Should().StartWith(@"
  Failed to create dummy of type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors:
    No Dummy Factory produced a result.
    It is not a Task.
    It is not a Lazy.
    It is not a value type.
    It has no public constructors.

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors.
      An exception of type Castle.DynamicProxy.InvalidProxyConstructorArgumentsException was caught during this call. Its message was:
      Can not instantiate proxy of class: FakeItEasy.Specs.DummyCreationSpecsBase+ClassWithNoPublicConstructors.
      Could not find a parameterless constructor."));
        }

        [Scenario]
        public void AvoidLongSelfReferentialConstructor(
            ClassWithLongSelfReferentialConstructor dummy1,
            ClassWithLongSelfReferentialConstructor dummy2)
        {
            "Given a class with multiple constructors"
                .See<ClassWithLongSelfReferentialConstructor>();

            "And the class has a one-parameter constructor not using its own type"
                .See(() => new ClassWithLongSelfReferentialConstructor(typeof(object)));

            "And the class has a two-parameter constructor using its own type"
                .See(() => new ClassWithLongSelfReferentialConstructor(typeof(object), default));

            "When I create a dummy of the class"
                .x(() => dummy1 = A.Dummy<ClassWithLongSelfReferentialConstructor>());

            "And I create another dummy of the class"
                .x(() => dummy2 = A.Dummy<ClassWithLongSelfReferentialConstructor>());

            "Then the first dummy is not null"
                .x(() => dummy1.Should().NotBeNull());

            "And it was created using the one-parameter constructor"
                .x(() => dummy1.NumberOfConstructorParameters.Should().Be(1));

            "And the second dummy is not null"
                .x(() => dummy2.Should().NotBeNull());

            "And it was created using the one-parameter constructor"
                .x(() => dummy2.NumberOfConstructorParameters.Should().Be(1));
        }

        public class ClassWithNoPublicConstructors
        {
            private ClassWithNoPublicConstructors()
            {
            }
        }

        public class ClassWithRecursiveDependency
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
            public ClassWithRecursiveDependency(AnIntermediateClassInTheRecursiveDependencyChain a)
            {
            }
        }

        public class AnIntermediateClassInTheRecursiveDependencyChain
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "c", Justification = "Required for testing.")]
            public AnIntermediateClassInTheRecursiveDependencyChain(ClassWithRecursiveDependency c)
            {
            }
        }

        public class ClassWithThrowingConstructor
        {
            public ClassWithThrowingConstructor()
            {
                throw new Exception("constructor threw");
            }
        }

        public class ClassWithNoResolvableConstructors
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "c", Justification = "Required for testing.")]
            public ClassWithNoResolvableConstructors(ClassWhoseDummyFactoryThrows c)
            {
            }
        }

        private abstract class PrivateAbstractClass
        {
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
