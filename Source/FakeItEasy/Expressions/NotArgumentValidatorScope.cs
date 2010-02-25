namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using System;
    using System.Text;

    internal class NotArgumentValidatorScope<T>
        : ArgumentValidatorScope<T>
    {
        public NotArgumentValidatorScope(ArgumentValidatorScope<T> parentValidations)
        {
            this.ParentValidations = parentValidations;
        }

        internal ArgumentValidatorScope<T> ParentValidations
        {
            get;
            private set;
        }

        internal override bool IsValid(T argument)
        {
            return this.ParentValidations.IsValid(argument);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            var parentDescription = this.ParentValidations.ToString();
            if (!string.IsNullOrEmpty(parentDescription))
            {
                result.Append(parentDescription);
                result.Append(" ");
            }

            result.Append("not");

            return result.ToString();
        }

        internal override bool ResultOfChildValidatorIsValid(bool result)
        {
            return !result;
        }
    }
}
