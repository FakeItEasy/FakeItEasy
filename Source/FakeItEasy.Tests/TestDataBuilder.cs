namespace FakeItEasy.Tests
{
    using System;

    public abstract class TestDataBuilder<TSubject, TBuilder> where TBuilder : TestDataBuilder<TSubject, TBuilder>
    {
        protected TestDataBuilder()
        {
        }

        public static TSubject Build(Action<TBuilder> buildAction)
        {
            var builder = CreateBuilderInstance();

            buildAction.Invoke(builder);

            return builder.Build();
        }

        public static TSubject BuildWithDefaults()
        {
            return Build(x => { });
        }

        public static implicit operator TSubject(TestDataBuilder<TSubject, TBuilder> builder)
        {
            return builder.Build();
        }

        protected abstract TSubject Build();

        protected TBuilder Do(Action<TBuilder> action)
        {
            action((TBuilder)this);
            return (TBuilder)this;
        }

        private static TBuilder CreateBuilderInstance()
        {
            return (TBuilder)Activator.CreateInstance(typeof(TBuilder), nonPublic: true);
        }
    }
}