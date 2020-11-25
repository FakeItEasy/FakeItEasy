namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

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

        [Scenario]
        public static void RaisingEvent(ClassWithVirtualEvent fake, Exception exception)
        {
            "Given a fake of a class with a virtual event that calls base methods"
                .x(() => fake = A.Fake<ClassWithVirtualEvent>(o => o.CallsBaseMethods()));

            "When the event is raised on the fake"
                .x(() => exception = Record.Exception(() => fake.Event += Raise.WithEmpty()));

            "Then it throws an InvalidOperationException"
                .x(() => exception.Should()
                    .BeAnExceptionOfType<InvalidOperationException>()
                    .WithMessage("*The fake cannot raise the event because event subscription calls the base implementation*"));
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

    public class ClassWithVirtualEvent
    {
#pragma warning disable CA1070 // Do not declare event fields as virtual
#pragma warning disable CS0067 // Event is never used
        public virtual event EventHandler? Event;
#pragma warning restore CS0067 // Event is never used
#pragma warning restore CA1070 // Do not declare event fields as virtual
    }
}
