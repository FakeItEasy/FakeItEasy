namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;

    public static class UnnaturalFakeSpecs
    {
        public interface IFoo
        {
            void VoidMethod();

            int MethodWithResult();
        }

        [Scenario]
        public static void CallsToVoidMethod(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure a void method to invoke an action"
                .x(() => fake.CallsTo(f => f.VoidMethod()).Invokes(() => wasCalled = true));

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.VoidMethod());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToIntMethod(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure an int-returning method to invoke an action"
                .x(() => fake.CallsTo(f => f.MethodWithResult()).Invokes(() => wasCalled = true));

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.MethodWithResult());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void AnyCall(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure any call to invoke an action"
                .x(() => fake.AnyCall().Invokes(() => wasCalled = true));

            "When I call a method on the faked object"
                .x(() => fake.FakedObject.MethodWithResult());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }
    }
}
