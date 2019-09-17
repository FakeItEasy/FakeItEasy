namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    internal class AnyCallCallRule
        : BuildableCallRule
    {
        private Func<ArgumentCollection, bool> argumentsPredicate = x => true;
        private Type applicableToMembersWithReturnType = typeof(AllReturnTypes);

        public void MakeApplicableToMembersWithReturnType(Type type) => this.applicableToMembersWithReturnType = type;

        public void MakeApplicableToAllNonVoidReturnTypes() => this.applicableToMembersWithReturnType = typeof(AllNonVoidReturnTypes);

        public override void DescribeCallOn(IOutputWriter writer)
        {
            if (this.applicableToMembersWithReturnType == typeof(AllNonVoidReturnTypes))
            {
                writer.Write("Any call with non-void return type to the fake object.");
            }
            else if (this.applicableToMembersWithReturnType == typeof(void))
            {
                writer.Write("Any call with void return type to the fake object.");
            }
            else if (this.applicableToMembersWithReturnType == typeof(AllReturnTypes))
            {
                writer.Write("Any call made to the fake object.");
            }
            else
            {
                writer.Write("Any call with return type ").Write(this.applicableToMembersWithReturnType).Write(" to the fake object.");
            }
        }

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> predicate)
        {
            this.argumentsPredicate = predicate;
        }

        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            try
            {
                if (!this.argumentsPredicate(fakeObjectCall.Arguments))
                {
                    return false;
                }
            }
            catch (Exception ex) when (!(ex is FakeConfigurationException))
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Arguments predicate"), ex);
            }

            if (this.applicableToMembersWithReturnType == typeof(AllReturnTypes))
            {
                return true;
            }

            if (this.applicableToMembersWithReturnType == typeof(AllNonVoidReturnTypes))
            {
                return fakeObjectCall.Method.ReturnType != typeof(void);
            }

            return this.applicableToMembersWithReturnType == fakeObjectCall.Method.ReturnType;
        }

        protected override BuildableCallRule CloneCallSpecificationCore() =>
            new AnyCallCallRule
            {
                argumentsPredicate = this.argumentsPredicate
            };

        private class AllNonVoidReturnTypes
        {
        }

        private class AllReturnTypes
        {
        }
    }
}
