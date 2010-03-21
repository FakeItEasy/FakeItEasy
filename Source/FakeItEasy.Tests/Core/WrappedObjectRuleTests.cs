using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Core;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;

namespace FakeItEasy.Tests.Core
{
    [TestFixture]
    public class WrappedObjectRuleTests
    {
        [Test]
        public void IsApplicableTo_should_return_true()
        {
            var rule = this.CreateRule();

            Assert.That(rule.IsApplicableTo(A.Fake<IFakeObjectCall>()));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_null()
        {
            var rule = this.CreateRule();

            Assert.That(rule.NumberOfTimesToCall, Is.Null);
        }

        [Test]
        public void Apply_should_set_return_value_from_wrapped_object([Values(0, 1, 2, 3, 5, 8)] int returnValue)
        {
            var wrapped = A.Fake<IFoo>();
            wrapped.Configure().CallsTo(x => x.Baz()).Returns(returnValue);

            var call = FakeCall.Create<IFoo>("Baz", new Type[] { }, new object[] { });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            Assert.That(call.ReturnValue, Is.EqualTo(returnValue));
        }

        [Test]
        public void Apply_should_use_arguments_from_call_when_calling_wrapped_object()
        {
            var wrapped = A.Fake<IFoo>();

            var call = FakeCall.Create<IFoo>("Bar", new Type[] { typeof(object), typeof(object) }, new object[] { "foo", "bar" });

            var rule = this.CreateRule(wrapped);
            rule.Apply(call);

            OldFake.Assert(wrapped).WasCalled(x => x.Bar("foo", "bar"));
        }

        private WrappedObjectRule CreateRule()
        {
            return new WrappedObjectRule(new object());
        }

        private WrappedObjectRule CreateRule(object wrapped)
        {
            return new WrappedObjectRule(wrapped);
        }
    }
}
