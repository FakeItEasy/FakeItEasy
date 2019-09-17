namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    internal class AnyCallCallRule
        : BuildableCallRule
    {
        private Func<ArgumentCollection, bool> argumentsPredicate;
        private bool applicableToAllNonVoidReturnTypes;
        private Type applicableToMembersWithReturnType;

        public AnyCallCallRule()
        {
            this.argumentsPredicate = x => true;
        }

        public void MakeApplicableToMembersWithReturnType(Type type) => this.applicableToMembersWithReturnType = type;

        public void MakeApplicableToAllNonVoidReturnTypes() => this.applicableToAllNonVoidReturnTypes = true;

        public override void DescribeCallOn(IOutputWriter writer)
        {
            if (this.applicableToMembersWithReturnType is object)
            {
                if (this.applicableToMembersWithReturnType == typeof(void))
                {
                    writer.Write("Any call with void return type to the fake object.");
                }
                else
                {
                    writer.Write("Any call with return type ").Write(this.applicableToMembersWithReturnType).Write(" to the fake object.");
                }
            }
            else if (this.applicableToAllNonVoidReturnTypes)
            {
                writer.Write("Any call with non-void return type to the fake object.");
            }
            else
            {
                writer.Write("Any call made to the fake object.");
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

            if (this.applicableToMembersWithReturnType is object)
            {
                return this.applicableToMembersWithReturnType == fakeObjectCall.Method.ReturnType;
            }

            if (this.applicableToAllNonVoidReturnTypes)
            {
                return fakeObjectCall.Method.ReturnType != typeof(void);
            }

            return true;
        }

        protected override BuildableCallRule CloneCallSpecificationCore() =>
            new AnyCallCallRule
            {
                argumentsPredicate = this.argumentsPredicate
            };
    }
}
