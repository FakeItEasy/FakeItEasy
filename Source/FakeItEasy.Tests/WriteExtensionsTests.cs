namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class WriteExtensionsTests : ConfigurableServiceLocatorTestBase
    {
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
            this.StubResolve<CallWriter>(callWriter);

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
            this.StubResolve<CallWriter>(callWriter);

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
            this.StubResolve<CallWriter>(callWriter);

            // Act
            calls.WriteToConsole();

            // Assert
            A.CallTo(() => callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>._, A<IOutputWriter>._)).MustHaveHappened();
        }
    }
}
