namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    internal class AnyCallCallRule
        : BuildableCallRule
    {
        private readonly FakeManager manager;
        private Func<ArgumentCollection, bool> argumentsPredicate = x => true;
        private Type applicableToMembersWithReturnType = typeof(AllReturnTypes);

        public AnyCallCallRule(FakeManager manager)
        {
            this.manager = manager;
        }

        public void MakeApplicableToMembersWithReturnType(Type type) => this.applicableToMembersWithReturnType = type;

        public void MakeApplicableToAllNonVoidReturnTypes() => this.applicableToMembersWithReturnType = typeof(AllNonVoidReturnTypes);

        public override void DescribeCallOn(IOutputWriter writer)
        {
            this.AppendCall(writer);
            this.AppendTargetObject(writer);
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
            new AnyCallCallRule(this.manager)
            {
                argumentsPredicate = this.argumentsPredicate
            };

        private void AppendCall(IOutputWriter writer)
        {
            writer.Write($"Any call {GetCallDescription()}");

            string GetCallDescription()
            {
                if (this.applicableToMembersWithReturnType == typeof(AllNonVoidReturnTypes))
                {
                    return "with non-void return type";
                }
                else if (this.applicableToMembersWithReturnType == typeof(void))
                {
                    return "with void return type";
                }
                else if (this.applicableToMembersWithReturnType == typeof(AllReturnTypes))
                {
                    return "made";
                }
                else
                {
                    return $"with return type {this.applicableToMembersWithReturnType}";
                }
            }
        }

        private void AppendTargetObject(IOutputWriter writer)
        {
            writer.Write($" to the fake object{GetTargetObjectName()}.");

            string GetTargetObjectName()
            {
                var fakeName = this.manager.FakeObjectName;
                if (string.IsNullOrEmpty(fakeName))
                {
                    return string.Empty;
                }

                return $" {fakeName}";
            }
        }

        private class AllNonVoidReturnTypes
        {
        }

        private class AllReturnTypes
        {
        }
    }
}
