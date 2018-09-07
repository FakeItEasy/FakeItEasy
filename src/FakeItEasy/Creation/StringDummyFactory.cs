namespace FakeItEasy
{
    internal class StringDummyFactory : DummyFactory<string>
    {
        public override Priority Priority => Priority.Internal;

        protected override string Create() => string.Empty;
    }
}
