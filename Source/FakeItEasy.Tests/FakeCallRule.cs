using System;
using NUnit.Framework;
using System.Linq.Expressions;
using FakeItEasy.Core;
using System.Reflection;
using FakeItEasy.Core;

namespace FakeItEasy.Tests
{
    public class FakeCallRule
    : IFakeObjectCallRule
    {
        public Func<IFakeObjectCall, bool> IsApplicableTo;
        public Action<IWritableFakeObjectCall> Apply;
        public bool ApplyWasCalled;
        public bool IsApplicableToWasCalled;

        bool IFakeObjectCallRule.IsApplicableTo(IFakeObjectCall invocation)
        {
            this.IsApplicableToWasCalled = true;
            return this.IsApplicableTo != null ? this.IsApplicableTo(invocation) : false;
        }

        void IFakeObjectCallRule.Apply(IWritableFakeObjectCall invocation)
        {
            this.ApplyWasCalled = true;

            if (this.Apply != null)
            {
                this.Apply(invocation);
            }
        }

        public int? NumberOfTimesToCall
        {
            get;
            set;
        }


        public bool MayNotBeCalledMoreThanTheNumberOfTimesSpecified
        {
            get;
            set;
        }


        public bool MustGetCalled
        {
            get;
            set;
        }
    }
}
