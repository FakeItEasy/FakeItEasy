namespace FakeItEasy.Creation
{
    using System;

    internal class FakeAndDummyManager
    {
        private readonly IFakeObjectCreator fakeCreator;
        private readonly IProxyOptionsFactory proxyOptionsFactory;
        private readonly IDummyValueResolver dummyValueResolver;

        public FakeAndDummyManager(IDummyValueResolver dummyValueResolver, IFakeObjectCreator fakeCreator, IProxyOptionsFactory proxyOptionsFactory)
        {
            this.dummyValueResolver = dummyValueResolver;
            this.fakeCreator = fakeCreator;
            this.proxyOptionsFactory = proxyOptionsFactory;
        }

        public object? CreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext)
        {
            return this.dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext).Result;
        }

        public object CreateFake(
            Type typeOfFake,
            LoopDetectingResolutionContext resolutionContext)
        {
            var proxyOptions = this.proxyOptionsFactory.BuildProxyOptions(typeOfFake, null);
            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.dummyValueResolver, resolutionContext).Result !;
        }

        public object CreateFake(
            Type typeOfFake,
            Action<IFakeOptions> optionsBuilder,
            LoopDetectingResolutionContext resolutionContext)
        {
            var proxyOptions = this.proxyOptionsFactory.BuildProxyOptions(typeOfFake, optionsBuilder);
            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.dummyValueResolver, resolutionContext).Result !;
        }

        public bool TryCreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext, out object? result)
        {
            var creationResult = this.dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext);
            if (creationResult.WasSuccessful)
            {
                result = creationResult.Result;
                return true;
            }

            result = default;
            return false;
        }
    }
}
