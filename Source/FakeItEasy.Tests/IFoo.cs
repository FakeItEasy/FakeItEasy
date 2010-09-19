using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;
using FakeItEasy.Configuration;

namespace FakeItEasy.Tests
{
    public interface IFoo
    {
        void Bar();
        void Bar(object argument);
        void Bar(object argument, object argument2);
        void Bar(object argument1, object argument2, object argument3);
        int Baz();
        int Baz(object argument, object argument2);
        object Biz();

        int SomeProperty { get; set; }
        string ReadOnlyProperty { get; }
        string WriteOnlyProperty { set; }

        IFoo ChildFoo { get; }

        int this[int index]
        {
            get;
            set;
        }

        event EventHandler SomethingHappened;
    }
}