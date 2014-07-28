namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class OutArgumentConstraint : IArgumentConstraint
    {
        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write("<out parameter>");
        }

        public bool IsValid(object argument)
        {
            return true;
        }
    }
}
