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
    using FakeItEasy.Tests.TestHelpers.FSharp;
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

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterface
        {
        }

        public interface IFakeCreator
        {
            Type FakeType { get; }

            object CreateFake(CreationSpecsBase specs);
        }

        [Scenario]
        [MemberData(nameof(SupportedTypes))]
        public void FakingSupportedTypes(IFakeCreator fakeCreator, object fake)
        {
            "Given a supported fake type"
                .See(fakeCreator.FakeType.ToString());

            "When I create a fake of the supported type"
                .x(() => fake = fakeCreator.CreateFake(this));

            "Then the result is a fake"
                .x(() => Fake.GetFakeManager(fake).Should().NotBeNull());

            "And the fake is of the correct type"
                .x(() => fake.Should().BeAssignableTo(fakeCreator.FakeType));
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

            "And the exception message indicates why construction failed"
                .x(() => exception.Message.Should().StartWithModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+ClassWhoseConstructorThrows:

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.CreationSpecsBase+ClassWhoseConstructorThrows.
      An exception of type System.NotSupportedException was caught during this call. Its message was:
      I don't like being constructed.
"));

            "And the exception message includes the original exception stack trace"
                .x(() => exception.Message.Should().Contain("FakeItEasy.Specs.CreationSpecsBase.ClassWhoseConstructorThrows..ctor()"));
        }

        [Scenario]
        public void FailureViaMultipleConstructors(
            Exception exception)
        {
            "Given a class with multiple constructors"
                .See<ClassWithMultipleConstructors>();

            "And one constructor throws"
                .See(() => new ClassWithMultipleConstructors());

            "And another constructor throws"
                .See(() => new ClassWithMultipleConstructors(string.Empty));

            "And a third constructor has an argument that cannot be resolved"
                .See(() => new ClassWithMultipleConstructors(new UnresolvableArgument(), string.Empty));

            "When I create a fake of the class"
                .x(() => exception = Record.Exception(() => this.CreateFake<ClassWithMultipleConstructors>()));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates why construction failed"
                .x(() => exception.Message.Should().MatchModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+ClassWithMultipleConstructors:

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.CreationSpecsBase+ClassWithMultipleConstructors.
      An exception of type System.Exception was caught during this call. Its message was:
      parameterless constructor failed
         *FakeItEasy.Specs.CreationSpecsBase.ClassWithMultipleConstructors..ctor()*

    Constructor with signature (System.String) failed:
      No constructor matches the passed arguments for constructor.
      An exception of type System.Exception was caught during this call. Its message was:
      string constructor failed
      with reason on two lines
         *FakeItEasy.Specs.CreationSpecsBase.ClassWithMultipleConstructors..ctor(String s)*

  The constructors with the following signatures were not tried:
    (*FakeItEasy.Specs.CreationSpecsBase+UnresolvableArgument, System.String)

    Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

"));
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
                .See(() => new FakedClass(new ArgumentThatShouldNeverBeResolved()));

            "And the class has a two-parameter constructor"
                .See(() => new FakedClass(A.Dummy<IDisposable>(), string.Empty));

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
                .See(() => new ClassWhosePreferredConstructorsThrow(A.Dummy<IDisposable>(), string.Empty));

            "And the class has a one-parameter constructor that succeeds"
                .See(() => new ClassWhosePreferredConstructorsThrow(default));

            // If multiple theads attempt to create the fake at the same time, the
            // unsuccessful constructors may be called more than once, so serialize fake
            // creation for this test.
            "And nobody else is trying to fake the class right now"
                .x(() => Monitor.TryEnter(ClassWhosePreferredConstructorsThrow.SyncRoot, TimeSpan.FromSeconds(30)).Should().BeTrue("we must enter the monitor"))
                .Teardown(() => Monitor.Exit(ClassWhosePreferredConstructorsThrow.SyncRoot));

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
            public static readonly object SyncRoot = new object();

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
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) is object));
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
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) is object));
        }

        [Scenario]
        [Example(2)]
        [Example(10)]
        public void CollectionOfFakeWithOptionBuilderCounter(
            int count,
            IList<ICollectionItem> fakes)
        {
            "When I create a collection of {0} fakes with unique names"
                .x(() => fakes = this.CreateCollectionOfFake<ICollectionItem>(count, (options, i) => options.Named($"Item{i}")));

            "Then {0} items are created"
                .x(() => fakes.Should().HaveCount(count));

            "And all items extend the specified type"
                .x(() => fakes.Should().ContainItemsAssignableTo<ICollectionItem>());

            "And all items are properly named"
                .x(() => fakes.Select(item => item.ToString()).Should().Equal(Enumerable.Range(0, count).Select(i => $"Item{i}")));

            "And all items are fakes"
                .x(() => fakes.Should().OnlyContain(item => Fake.GetFakeManager(item) is object));
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
        public void PrivateClassCannotBeFaked(Exception exception)
        {
            "Given a private class"
                .See<PrivateClass>();

            "When I create a fake of the class"
                .x(() => exception = Record.Exception(this.CreateFake<PrivateClass>));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().MatchModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+PrivateClass:

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.CreationSpecsBase+PrivateClass.
      An exception of type Castle.DynamicProxy.Generators.GeneratorException was caught during this call. Its message was:
      Can not create proxy for type FakeItEasy.Specs.CreationSpecsBase+PrivateClass because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)] attribute*
"));
        }

        [Scenario]
        public void PrivateDelegateCannotBeFaked(Exception exception)
        {
            "Given a private delegate"
                .See<PrivateDelegate>();

            "When I create a fake of the delegate"
                .x(() => exception = Record.Exception(this.CreateFake<PrivateDelegate>));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().MatchModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+PrivateDelegate:
    Can not create proxy for type FakeItEasy.Specs.CreationSpecsBase+PrivateDelegate because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)] attribute*
"));
        }

        [Scenario]
        public void PublicDelegateWithPrivateTypeArgumentCannotBeFaked(Exception exception)
        {
            "Given a public delegate with a private type argument"
                .See<Func<PrivateClass>>();

            "When I create a fake of the delegate"
                .x(() => exception = Record.Exception(this.CreateFake<Func<PrivateClass>>));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().MatchModuloLineEndings(@"
  Failed to create fake of type System.Func`1[FakeItEasy.Specs.CreationSpecsBase+PrivateClass]:
    Can not create proxy for type System.Func`1[[FakeItEasy.Specs.CreationSpecsBase+PrivateClass, FakeItEasy.Specs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=eff28e2146d5fd2c]] because type FakeItEasy.Specs.CreationSpecsBase+PrivateClass is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)] attribute*
"));
        }

        [Scenario]
        public void PublicDelegateWithAnonymousParameterCanBeFaked(IAmADelegateWithAnAnonymousParameter fake)
        {
            "Given a public delegate with an anonymous parameter "
                .See<IAmADelegateWithAnAnonymousParameter>();

            "When I create a fake of the delegate"
                .x(() => fake = this.CreateFake<IAmADelegateWithAnAnonymousParameter>());

            "Then the fake is created"
                .x(() => fake.Should().BeAFake());
        }

        [Scenario]
        public void ClassWithPrivateConstructorCannotBeFaked(Exception exception)
        {
            "Given a class with a private constructor"
                .See<ClassWithPrivateConstructor>();

            "When I create a fake of the class"
                .x(() => exception = Record.Exception(this.CreateFake<ClassWithPrivateConstructor>));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().StartWithModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+ClassWithPrivateConstructor:

  Below is a list of reasons for failure per attempted constructor:
    Constructor with signature () failed:
      No usable default constructor was found on the type FakeItEasy.Specs.CreationSpecsBase+ClassWithPrivateConstructor.
      An exception of type Castle.DynamicProxy.InvalidProxyConstructorArgumentsException was caught during this call. Its message was:
      Can not instantiate proxy of class: FakeItEasy.Specs.CreationSpecsBase+ClassWithPrivateConstructor.
      Could not find a parameterless constructor.
"));
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
                .x(() => exception.Message.Should().BeModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+SealedClass:
    The type of proxy FakeItEasy.Specs.CreationSpecsBase+SealedClass is sealed.
"));
        }

        [Scenario]
        public void CannotFakeInterfaceWithConstructorArguments(Exception exception)
        {
            "Given a fakeable interface"
                .See<IInterface>();

            "When I create a fake of the interface supplying constructor arguments"
                .x(() => exception = Record.Exception(() => this.CreateFake<IInterface>(options =>
                    options.WithArgumentsForConstructor(new object[] { 7 }))));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<ArgumentException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().Be("Arguments for constructor specified for interface type."));
        }

        [Scenario]
        public void SuppliedConstructorArgumentsArePassedToClassConstructor(AClassThatCouldBeFakedWithTheRightConstructorArguments fake)
        {
            "Given a fakeable class"
                .See<AClassThatCouldBeFakedWithTheRightConstructorArguments>();

            "When I create a fake of the class supplying valid constructor arguments"
                .x(() => fake = this.CreateFake<AClassThatCouldBeFakedWithTheRightConstructorArguments>(options =>
                    options.WithArgumentsForConstructor(() => new AClassThatCouldBeFakedWithTheRightConstructorArguments(17))));

            "Then it passes the supplied arguments to the constructor"
                .x(() => fake.ID.Should().Be(17));
        }

        [Scenario]
        public void CannotFakeWithBadConstructorArguments(Exception exception)
        {
            "Given a fakeable class"
                .See<AClassThatCouldBeFakedWithTheRightConstructorArguments>();

            "When I create a fake of the class supplying invalid constructor arguments"
                .x(() => exception = Record.Exception(() => this.CreateFake<AClassThatCouldBeFakedWithTheRightConstructorArguments>(options =>
                    options.WithArgumentsForConstructor(new object[] { 7, "magenta" }))));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().StartWithModuloLineEndings(@"
  Failed to create fake of type FakeItEasy.Specs.CreationSpecsBase+AClassThatCouldBeFakedWithTheRightConstructorArguments:
    No constructor matches the passed arguments for constructor.
    An exception of type Castle.DynamicProxy.InvalidProxyConstructorArgumentsException was caught during this call. Its message was:
    Can not instantiate proxy of class: FakeItEasy.Specs.CreationSpecsBase+AClassThatCouldBeFakedWithTheRightConstructorArguments.
    Could not find a constructor that would match given arguments:
    System.Int32
    System.String"));
        }

        [Scenario]
        public void FakeDelegateCreation(Func<int> fake)
        {
            "Given a delegate"
                .See<Func<int>>();

            "When I create a fake of the delegate"
                .x(() => fake = this.CreateFake<Func<int>>());

            "Then it creates the fake"
                .x(() => fake.Should().NotBeNull());
        }

        [Scenario]
        public void FakeDelegateCreationWithAttributes(Exception exception)
        {
            "Given a delegate"
                .See<Func<int>>();

            "When I create a fake of the delegate with custom attributes"
                .x(() => exception = Record.Exception(() => this.CreateFake<Func<int>>(options => options.WithAttributes(() => new ObsoleteAttribute()))));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().BeModuloLineEndings(@"
  Failed to create fake of type System.Func`1[System.Int32]:
    Faked delegates cannot have custom attributes applied to them.
"));
        }

        [Scenario]
        public void FakeDelegateCreationWithArgumentsForConstructor(Exception exception)
        {
            "Given a delegate"
                .See<Func<int>>();

            "When I create a fake of the delegate using explicit constructor arguments"
                .x(() => exception = Record.Exception(() => this.CreateFake<Func<int>>(options => options.WithArgumentsForConstructor(new object[] { 7 }))));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().BeModuloLineEndings(@"
  Failed to create fake of type System.Func`1[System.Int32]:
    Faked delegates cannot be made using explicit constructor arguments.
"));
        }

        [Scenario]
        public void FakeDelegateCreationWithAdditionalInterfaces(Exception exception)
        {
            "Given a delegate"
                .See<Func<int>>();

            "When I create a fake of the delegate with additional implemented interfaces"
                .x(() => exception = Record.Exception(() => this.CreateFake<Func<int>>(options => options.Implements<IList<string>>())));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should().BeModuloLineEndings(@"
  Failed to create fake of type System.Func`1[System.Int32]:
    Faked delegates cannot be made to implement additional interfaces.
"));
        }

        [Scenario]
        public void NamedFakeToString(object fake, string? toStringResult)
        {
            "Given a named fake"
                .x(() => fake = A.Fake<object>(o => o.Named("Foo")));

            "When I call ToString() on the fake"
                .x(() => toStringResult = fake.ToString());

            "Then it returns the configured name of the fake"
                .x(() => toStringResult.Should().Be("Foo"));
        }

        [Scenario]
        public void NamedFakeAsArgument(object fake, Action<object> fakeAction, Exception exception)
        {
            "Given a named fake"
                .x(() => fake = A.Fake<object>(o => o.Named("Foo")));

            "And a fake action that takes an object as the parameter"
                .x(() => fakeAction = A.Fake<Action<object>>());

            "When I assert that the fake action was called with the named fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fakeAction(fake)).MustHaveHappened()));

            "Then the exception message describes the named fake by its name"
                .x(() => exception.Message.Should().Contain(".Invoke(obj: Foo)"));
        }

        [Scenario]
        public void NamedFakeDelegateToString(Action fake, string? toStringResult)
        {
            "Given a named delegate fake"
                .x(() => fake = A.Fake<Action>(o => o.Named("Foo")));

            "When I call ToString() on the fake"
                .x(() => toStringResult = fake.ToString());

            "Then it returns the name of the faked object type, because ToString() can't be faked for a delegate"
                .x(() => toStringResult.Should().Be("System.Action"));
        }

        [Scenario]
        public void NamedFakeDelegateAsArgument(Action fake, Action<Action> fakeAction, Exception exception)
        {
            "Given a named delegate fake"
                .x(() => fake = A.Fake<Action>(o => o.Named("Foo")));

            "And a fake action that takes an action as the parameter"
                .x(() => fakeAction = A.Fake<Action<Action>>());

            "When I assert that the fake action was called with the named fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fakeAction(fake)).MustHaveHappened()));

            "Then the exception message describes the named fake by its name"
                .x(() => exception.Message.Should().Contain(".Invoke(obj: Foo)"));
        }

        [Scenario]
        public void NamedFakeExceptionMessage(Action fake, Exception exception)
        {
            "Given a named delegate fake"
                .x(() => fake = A.Fake<Action>(o => o.Named("Foo1")));

            "When I assert that the fake action was called"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake()).MustHaveHappened()));

            "Then the exception message describes the named fake by its name"
                .x(() => exception.Message.Should().Contain("System.Action.Invoke() on Foo1"));
        }

        [Scenario]
        public void AvoidLongSelfReferentialConstructor(
            ClassWithLongSelfReferentialConstructor fake1,
            ClassWithLongSelfReferentialConstructor fake2)
        {
            "Given a class with multiple constructors"
                .See<ClassWithLongSelfReferentialConstructor>();

            "And the class has a one-parameter constructor not using its own type"
                .See(() => new ClassWithLongSelfReferentialConstructor(typeof(object)));

            "And the class has a two-parameter constructor using its own type"
                .See(() => new ClassWithLongSelfReferentialConstructor(typeof(object), A.Dummy<ClassWithLongSelfReferentialConstructor>()));

            "When I create a fake of the class"
                .x(() => fake1 = A.Fake<ClassWithLongSelfReferentialConstructor>());

            "And I create another fake of the class"
                .x(() => fake2 = A.Fake<ClassWithLongSelfReferentialConstructor>());

            "Then the first fake is not null"
                .x(() => fake1.Should().NotBeNull());

            "And it was created using the one-parameter constructor"
                .x(() => fake1.NumberOfConstructorParameters.Should().Be(1));

            "And the second fake is not null"
                .x(() => fake2.Should().NotBeNull());

            "And it was created using the one-parameter constructor"
                .x(() => fake2.NumberOfConstructorParameters.Should().Be(1));
        }

        protected abstract T CreateFake<T>() where T : class;

        protected abstract T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder) where T : class;

        protected abstract IList<T> CreateCollectionOfFake<T>(int numberOfFakes) where T : class;

        protected abstract IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder) where T : class;

        protected abstract IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>, int> optionsBuilder) where T : class;

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

        public class AClassThatCouldBeFakedWithTheRightConstructorArguments
        {
            public AClassThatCouldBeFakedWithTheRightConstructorArguments(int id)
            {
                this.ID = id;
            }

            public int ID { get; }
        }

        public sealed class UnresolvableArgument
        {
            public UnresolvableArgument() => throw new InvalidOperationException();
        }

        public class ClassWithMultipleConstructors
        {
            public ClassWithMultipleConstructors() => throw new Exception("parameterless constructor failed");

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "s", Justification = "Required for testing.")]
            public ClassWithMultipleConstructors(string s) => throw new Exception("string constructor failed" + Environment.NewLine + "with reason on two lines");

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "u", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "s", Justification = "Required for testing.")]
            public ClassWithMultipleConstructors(UnresolvableArgument u, string s)
            {
            }
        }

        public abstract class AbstractClass
        {
        }

        public class ClassWithProtectedConstructor
        {
            protected ClassWithProtectedConstructor()
            {
            }
        }

        public class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }

        private static IEnumerable<object?[]> SupportedTypes() => TestCases.FromObject(
            new FakeCreator<IInterface>(),
            new FakeCreator<AbstractClass>(),
            new FakeCreator<ClassWithProtectedConstructor>(),
            new FakeCreator<ClassWithInternalConstructorVisibleToDynamicProxy>(),
            new FakeCreator<InternalClassVisibleToDynamicProxy>(),
            new FakeCreator<Action>(),
            new FakeCreator<Func<int, string>>(),
            new FakeCreator<EventHandler<EventArgs>>());

        private class FakeCreator<TFake> : IFakeCreator where TFake : class
        {
            public Type FakeType => typeof(TFake);

            public object CreateFake(CreationSpecsBase testRunner)
            {
                return testRunner.CreateFake<TFake>();
            }

            public override string ToString() => typeof(TFake).Name;
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Required for testing.")]
        private class PrivateClass
        {
        }

        private delegate void PrivateDelegate();
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

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>, int> optionsBuilder)
        {
            return A.CollectionOfFake(numberOfFakes, optionsBuilder);
        }
    }

    public class NonGenericCreationSpecs : CreationSpecsBase
    {
        [Scenario]
        public void StructCannotBeFaked(Exception exception)
        {
            "Given a struct"
                .See<Struct>();

            "When I create a fake of the struct"
                .x(() => exception = Record.Exception(() => (Struct)Sdk.Create.Fake(typeof(Struct))));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should()
                    .Be("Failed to create Fake of type FakeItEasy.Specs.CreationSpecsBase+Struct because it's a value type."));
        }

        [Scenario]
        public void StructCannotBeFakedWithOptions(Exception exception)
        {
            "Given a struct"
                .See<Struct>();

            "When I create a fake of the struct supplying fake creation options"
                .x(() => exception = Record.Exception(() => (Struct)Sdk.Create.Fake(typeof(Struct), options => { })));

            "Then it throws a fake creation exception"
                .x(() => exception.Should().BeOfType<FakeCreationException>());

            "And the exception message indicates the reason for failure"
                .x(() => exception.Message.Should()
                    .Be("Failed to create Fake of type FakeItEasy.Specs.CreationSpecsBase+Struct because it's a value type."));
        }

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

        protected override IList<T> CreateCollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>, int> optionsBuilder)
        {
            return Sdk.Create.CollectionOfFake(typeof(T), numberOfFakes, (options, i) => optionsBuilder((IFakeOptions<T>)options, i)).Cast<T>().ToList();
        }
    }
}
