namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;

    public static class CallsBaseMethodsFakeSpecs
    {
        [Scenario]
        public static void ConcreteMethod(
            AbstractBaseClass fake,
            string result)
        {
            "establish"
                .x(() => fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods()));

            "when concrete method is called on fake that calls base methods"
                .x(() => result = fake.ConcreteMethod());

            "it should call base method"
                .x(() => result.Should().Be("result from base method"));
        }

        [Scenario]
        public static void AbstractMethod(
            AbstractBaseClass fake,
            string result)
        {
            "establish"
                .x(() => fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods()));

            "when abstract method is called on fake that calls base methods"
                .x(() => result = fake.AbstractMethod());

            "it should return default value"
                .x(() => result.Should().BeEmpty());
        }
    }

    public abstract class AbstractBaseClass
    {
        public virtual string ConcreteMethod()
        {
            return "result from base method";
        }

        public abstract string AbstractMethod();
    }
}
