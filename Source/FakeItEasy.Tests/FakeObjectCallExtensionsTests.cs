namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeObjectCallExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void GetArgument_should_delegate_to_the_argument_collections_get_method_when_using_index()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>(0);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void GetArgument_should_delegate_to_the_argument_collections_get_method_when_using_name()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>("argument2");

            // Assert
            result.Should().Be(2);
        }

        [Test]
        public void GetArgument_should_be_null_guarded_when_using_index()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFakeObjectCall>().GetArgument<int>(0));
        }

        [Test]
        public void GetArgument_should_be_null_guarded_when_using_argument_name()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFakeObjectCall>().GetArgument<int>("foo"));
        }

        [Test]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            // Arrange
            var call = FakeCall.Create<object>("GetType");

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("System.Object.GetType()");
        }

        [Test]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar("abc", 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(\"abc\", 123)");
        }

        [Test]
        public void GetDescription_should_render_null_when_argument_is_null()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar(null, 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<NULL>, 123)");
        }

        [Test]
        public void GetDescription_should_render_string_empty_when_string_is_empty()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar(string.Empty, 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<string.Empty>, 123)");
        }

        [Test]
        public void Write_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                Enumerable.Empty<IFakeObjectCall>().Write(A.Dummy<IOutputWriter>()));
        }

        [Test]
        public void Write_should_call_writer_registered_in_container_with_calls()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve(callWriter);

            var writer = A.Dummy<IOutputWriter>();

            // Act
            calls.Write(writer);

            // Assert
            A.CallTo(() => callWriter.WriteCalls(calls, writer)).MustHaveHappened();
        }

        [Test]
        public void WriteToConsole_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                Enumerable.Empty<IFakeObjectCall>().WriteToConsole());
        }

        [Test]
        public void WriteToConsole_should_call_writer_registered_in_container_with_calls()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve(callWriter);

            // Act
            calls.WriteToConsole();

            // Assert
            A.CallTo(() => callWriter.WriteCalls(calls, A<IOutputWriter>._)).MustHaveHappened();
        }

        [Test]
        public void WriteToConsole_should_call_writer_registered_in_container_with_console_out()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve(callWriter);

            // Act
            calls.WriteToConsole();

            // Assert
            A.CallTo(() => callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>._, A<IOutputWriter>._)).MustHaveHappened();
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        {
            return FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) }, new[] { argument1, argument2 });
        }
    }
}
