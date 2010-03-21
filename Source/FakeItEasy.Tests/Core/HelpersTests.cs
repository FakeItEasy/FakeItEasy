using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using FakeItEasy.Configuration;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.Core
{
    [TestFixture]
    public class HelpersTests
        : ConfigurableServiceLocatorTestBase
    {
        protected override void OnSetUp()
        {
            
        }

        [Test]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            var call = FakeCall.Create<object>("GetType");

            Assert.That(Helpers.GetDescription(call), Is.EqualTo("System.Object.GetType()"));
        }

        [Test]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            var call = CreateFakeCallToFooDotBar("abc", 123);

            Assert.That(Helpers.GetDescription(call), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(\"abc\", 123)"));
        }

        [Test]
        public void GetDescription_should_render_NULL_when_argument_is_null()
        {
            var call = CreateFakeCallToFooDotBar(null, 123);

            Assert.That(Helpers.GetDescription(call), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<NULL>, 123)"));
        }

        [Test]
        public void GetDescription_should_render_string_empty_when_string_is_empty()
        {
            var call = CreateFakeCallToFooDotBar("", 123);

            Assert.That(Helpers.GetDescription(call), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<string.Empty>, 123)"));
        }

        [Test]
        public void WriteCalls_should_throw_when_calls_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Helpers.WriteCalls((IEnumerable<IFakeObjectCall>)null, new StringWriter()));
        }

        [Test]
        public void WriteCalls_should_call_call_writer_registered_in_container_with_calls()
        {
            var calls = new List<IFakeObjectCall> 
            {
                CreateFakeCallToFooDotBar("abc", 123),
                CreateFakeCallToFooDotBar("def", 456)
            };

            var callWriter = A.Fake<CallWriter>();
            this.StubResolve<CallWriter>(callWriter);


            var writer = new StringWriter();

            Helpers.WriteCalls(calls, writer);


            OldFake.Assert(callWriter)
                .WasCalled(x => x.WriteCalls(0, calls, writer));
        }

        private class Base
        {
            public virtual void BaseMethod()
            { 
            
            }

            public void BaseNonVirtualMethod()
            {
            
            }
        }

        private class Middle
            : Base, IHideObjectMembers
        {
 
        }


        private class Derived
            : Middle
        {
            public override void BaseMethod()
            {
                base.BaseMethod();
            }

            public new void BaseNonVirtualMethod()
            { }
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        { 
            var call = FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) },
                new[] { argument1, argument2 });

            return call;
        }

        private IEnumerable<IFakeObjectCall> GetStubCalls()
        {
            return new List<IFakeObjectCall> 
            {
                CreateFakeCallToFooDotBar("abc", 123),
                CreateFakeCallToFooDotBar("def", 456)
            };
        }
    }
}
