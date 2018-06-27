namespace FakeItEasy.Core
{
    using System;

    internal sealed class CallCountConstraint
    {
        private readonly Func<int, bool> predicate;
        private readonly string description;

        public CallCountConstraint(Func<int, bool> predicate, string description)
        {
            this.predicate = predicate;
            this.description = description;
        }

        public bool Matches(int callCount)
        {
            try
            {
                return this.predicate.Invoke(callCount);
            }
            catch (Exception ex)
            {
                throw new UserCallbackException($"Call count constraint <{this.description}> threw an exception. See inner exception for details.", ex);
            }
        }

        public override string ToString()
        {
            return this.description;
        }
    }
}
