namespace FakeItEasy.Specs
{
    using Machine.Specifications;

    public class when_matching_calls_with_param_arrays
    {
        static ITypeWithParamArray fake;

        Establish context = () => fake = A.Fake<ITypeWithParamArray>();

        Because of = () => fake.MethodWithParamArray("foo", "bar", "baz");

        It should_be_able_to_match_the_call =
            () => A.CallTo(() => fake.MethodWithParamArray("foo", "bar", "baz")).MustHaveHappened();

        It should_be_able_to_match_the_call_with_argument_constraints = 
            () => A.CallTo(() => fake.MethodWithParamArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened();

        It should_be_able_to_match_using_array_syntax =
            () => A.CallTo(() => fake.MethodWithParamArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened();

        public interface ITypeWithParamArray
        {
            void MethodWithParamArray(string arg, params string[] args);
        }
    }
}