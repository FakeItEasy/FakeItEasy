namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        /// <summary>
        /// Create a fake.
        /// </summary>
        /// <param name="typeOfFake">The type of the fake.</param>
        /// <param name="proxyOptions">Proxy options to be used when creating the fake.</param>
        /// <param name="resolver">Will be used to create dummy constructor arguments, if needed.</param>
        /// <param name="resolutionContext">Used to detect loops in the type resolution.</param>
        /// <returns>A <c>CreationResult</c> that contains the fake or indicates the reason for failure.</returns>
        CreationResult CreateFake(
            Type typeOfFake,
            IProxyOptions proxyOptions,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext);

        /// <summary>
        /// Create a fake, without first checking to see if the <paramref name="resolutionContext"/> indicates
        /// that we're already trying to create a fake <paramref name="typeOfFake"/>.
        /// </summary>
        /// <remarks>
        /// <para>Normal type resolution loop detection is avoided only for this attempt to create
        /// <paramref name="typeOfFake"/>. Loop detection is still on for any constructor parameters
        /// required to make the fake, even if they are of type <c>typeOfFake</c> (or if those
        /// parameters in turn require a <c>typeOfFake</c> and so on).</para>
        /// <para>This method should only be used when it is known that attempting to create a fake
        /// <c>typeOfFake</c> will not in and of itself introduce a loop, for example when
        /// creating a fake that is to be used as a dummy, the <see cref="DummyValueResolver"/>
        /// checks that it's safe to make a fake before using
        /// <see cref="DummyValueResolver.ResolveByCreatingFakeStrategy"/> to do so.</para>
        /// <para>In most cases, prefer <see cref="CreateFake"/>.</para>
        /// </remarks>
        /// <param name="typeOfFake">The type of the fake.</param>
        /// <param name="proxyOptions">Proxy options to be used when creating the fake.</param>
        /// <param name="resolver">Will be used to create dummy constructor arguments, if needed.</param>
        /// <param name="resolutionContext">Used to detect loops in the type resolution.</param>
        /// <returns>A <c>CreationResult</c> that contains the fake or indicates the reason for failure.</returns>
        CreationResult CreateFakeWithoutLoopDetection(
            Type typeOfFake,
            IProxyOptions proxyOptions,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext);
    }
}
