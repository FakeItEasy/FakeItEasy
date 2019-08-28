namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using Xunit;

    public class FakeObjectCreatorTests
    {
        private readonly FakeObjectCreator fakeObjectCreator;
        private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;

        public FakeObjectCreatorTests()
        {
            this.fakeCallProcessorProviderFactory = A.Fake<FakeCallProcessorProvider.Factory>();

            this.fakeObjectCreator = new FakeObjectCreator(
                this.fakeCallProcessorProviderFactory,
                A.Dummy<IMethodInterceptionValidator>(),
                A.Dummy<IMethodInterceptionValidator>());
        }

        [Fact]
        public void Should_use_new_fake_call_processor_for_every_tried_constructor()
        {
            // Arrange
            var dummyValueResolver = A.Fake<IDummyValueResolver>();
            StubResolverWithDummyValue(dummyValueResolver, 1);
            StubResolverWithDummyValue(dummyValueResolver, "dummy");

            var options = new ProxyOptions();

            // Act
            this.fakeObjectCreator.CreateFake(typeof(TypeWithMultipleConstructors), options, dummyValueResolver);

            // Assert
            A.CallTo(() => this.fakeCallProcessorProviderFactory(typeof(TypeWithMultipleConstructors), options))
                .MustHaveHappened(3, Times.Exactly);
        }

        private static void StubResolverWithDummyValue<T>(IDummyValueResolver resolver, T dummyValue)
        {
            A.CallTo(() => resolver.TryResolveDummyValue(typeof(T), A<LoopDetectingResolutionContext>._))
                .Returns(CreationResult.SuccessfullyCreated(dummyValue));
        }

        public class TypeWithMultipleConstructors
        {
            public TypeWithMultipleConstructors()
            {
                throw new NotImplementedException();
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            public TypeWithMultipleConstructors(string argument1)
            {
                throw new NotImplementedException();
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithMultipleConstructors(int argument1, int argument2)
            {
                throw new NotImplementedException();
            }
        }
    }
}
