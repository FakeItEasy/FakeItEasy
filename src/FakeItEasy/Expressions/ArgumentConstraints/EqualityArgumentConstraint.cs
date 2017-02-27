namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System;
    using FakeItEasy.Core;

    internal class EqualityArgumentConstraint
        : IArgumentConstraint
    {
        public EqualityArgumentConstraint(object expectedValue)
        {
            this.ExpectedValue = expectedValue;
        }

        public object ExpectedValue { get; }

        public string ConstraintDescription => this.ToString();

        public bool IsValid(object argument)
        {
            return object.Equals(this.ExpectedValue, argument);
        }

        public override string ToString()
        {
            if (this.ExpectedValue == null)
            {
                return "<NULL>";
            }

            var stringValue = this.ExpectedValue as string;
            if (stringValue != null)
            {
                return "\"{0}\"".FormatInvariant(stringValue);
            }

            try
            {
                return this.ExpectedValue.ToString();
            }
            catch (Exception)
            {
                FakeManager manager = Fake.TryGetFakeManager(this.ExpectedValue);
                return manager != null
                    ? $"Faked {manager.FakeObjectType.FullName}"
                    : this.ExpectedValue.GetType().ToString();
            }
        }

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write(this.ConstraintDescription);
        }
    }
}
