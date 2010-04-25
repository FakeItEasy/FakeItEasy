namespace FakeItEasy.Expressions
{
    internal class RootArgumentConstraintScope<T>
        : ArgumentConstraintScope<T>
    {
        internal override bool IsValid(T argument)
        {
            return true;
        }

        internal override bool ResultOfChildConstraintIsValid(bool result)
        {
            return result;
        }
    }
}
