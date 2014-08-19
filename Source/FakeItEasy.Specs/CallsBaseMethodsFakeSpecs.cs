namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Machine.Specifications;

    public abstract class AbstractBaseClass
    {
        public virtual string ConcreteMethod()
        {
            return "result from base method";
        }

        public abstract string AbstractMethod();
    }

    public class when_concrete_method_is_called_on_fake_that_calls_base_methods
    {
        static AbstractBaseClass fake;
        static string result;

        Establish context = () =>
        {
            fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods());
        };

        Because of = () => result = fake.ConcreteMethod();

        It should_call_base_method = () => result.Should().Be("result from base method");
    }

    public class when_abstract_method_is_called_on_fake_that_calls_base_methods
    {
        static AbstractBaseClass fake;
        static string result = "some non-default value";

        Establish context = () =>
        {
            fake = A.Fake<AbstractBaseClass>(options => options.CallsBaseMethods());
        };

        Because of = () => result = fake.AbstractMethod();

        It should_return_default_value = () => result.Should().BeEmpty();
    }
}
