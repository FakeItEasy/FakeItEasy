namespace FakeItEasy.Core
{
    internal interface IArgumentProvider
    {
        object[] GetArguments(object fake);
    }
}