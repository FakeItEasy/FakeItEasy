namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using System;
    using System.Text;

    internal class NotArgumentConstraintScope<T>
        : ArgumentConstraintScope<T>
    {
        public NotArgumentConstraintScope(ArgumentConstraintScope<T> parentValidations)
        {
            this.ParentValidations = parentValidations;
        }

        internal ArgumentConstraintScope<T> ParentValidations
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

        internal override bool ResultOfChildConstraintIsValid(bool result)
        {
            return !result;
        }
    }
}
