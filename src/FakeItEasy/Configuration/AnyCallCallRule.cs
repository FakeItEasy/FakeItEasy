namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    internal class AnyCallCallRule
        : BuildableCallRule
    {
        private Func<ArgumentCollection, bool> argumentsPredicate;

        public AnyCallCallRule()
        {
            this.argumentsPredicate = x => true;
        }

        public Type ApplicableToMembersWithReturnType { get; set; }

        public bool ApplicableToAllNonVoidReturnTypes { get; set; }

        public override void DescribeCallOn(IOutputWriter writer)
        {
            if (this.ApplicableToMembersWithReturnType != null)
            {
                if (this.ApplicableToMembersWithReturnType == typeof(void))
                {
                    writer.Write("Any call with void return type to the fake object.");
                }
                else
                {
                    writer.Write("Any call with return type ").Write(this.ApplicableToMembersWithReturnType).Write(" to the fake object.");
                }
            }
            else if (this.ApplicableToAllNonVoidReturnTypes)
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
            if (!this.argumentsPredicate(fakeObjectCall.Arguments))
            {
                return false;
            }

            if (this.ApplicableToMembersWithReturnType != null)
            {
                return this.ApplicableToMembersWithReturnType == fakeObjectCall.Method.ReturnType;
            }

            if (this.ApplicableToAllNonVoidReturnTypes)
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
