namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;

    public abstract class AbstractBaseClass
    {
        public virtual string ConcreteMethod()
        {
            return "result from base method";
        }

        public abstract string AbstractMethod();
    }

    public class CallsBaseMethodsFake
    {
        [Scenario]
        public void when_concrete_method_is_called_on_fake_that_calls_base_methods(
            AbstractBaseClass fake,
            string result)
        {
            "establish".x(() =>
            {
                fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods());
            });

            "when concrete method is called on fake that calls base methods".x(() =>
            {
                result = fake.ConcreteMethod();
            });

            "it should call base method".x(() =>
            {
                result.Should().Be("result from base method");
            });
        }

        [Scenario]
        public void when_abstract_method_is_called_on_fake_that_calls_base_methods(
            AbstractBaseClass fake,
            string result = "some non-default value")
        {
            "establish".x(() =>
            {
                fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods());
            });

            "when concrete method is called on fake that calls base methods".x(() =>
            {
                result = fake.AbstractMethod();
            });

            "it should return default value".x(() =>
            {
                result.Should().BeEmpty();
            });
        }
    }
}
