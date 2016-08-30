namespace FakeItEasy.Tests
{
    public interface IOutputAndRef
    {
        int Foo(int numberIn, string textIn, out int numberOut, ref string textReference);

        int Bar(int numberIn, string textIn, out int numberOut, ref string textReference);
    }
}
