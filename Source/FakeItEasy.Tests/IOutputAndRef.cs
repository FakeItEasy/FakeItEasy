namespace FakeItEasy.Tests
{
    using System.Diagnostics.CodeAnalysis;

    public interface IOutputAndRef
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for testing.")]
        int Foo(int numberIn, string textIn, out int numberOut, ref string textReference);

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for testing.")]
        int Bar(int numberIn, string textIn, out int numberOut, ref string textReference);
    }
}
