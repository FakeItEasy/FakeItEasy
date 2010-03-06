using NUnit.Framework;
using FakeItEasy.Core;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class CallInterceptedEventArgsTests
    {
        [Test]
        public void CallInterceptedEventArgs_should_be_serializable()
        {
            Assert.That(new CallInterceptedEventArgs(A.Fake<IWritableFakeObjectCall>()), Is.BinarySerializable);
        }

        [Test]
        public void Constructor_should_set_call()
        {
            var call = A.Fake<IWritableFakeObjectCall>();

            var args = new CallInterceptedEventArgs(call);

            Assert.That(args.Call, Is.SameAs(call));
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new CallInterceptedEventArgs(A.Fake<IWritableFakeObjectCall>()));
        }
    }
}
