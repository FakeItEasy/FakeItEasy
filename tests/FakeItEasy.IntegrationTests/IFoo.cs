namespace FakeItEasy.IntegrationTests
{
    public interface IFoo
    {
        void Bar();

        void Bar(object argument);

        void Bar(object argument, object argument2);

        void Bar(object argument1, object argument2, object argument3);

        int Baz();

        int Baz(object argument, object argument2);

        object Biz();

        int IntProperty { get; }

        string StringProperty { get; set; }
    }
}
