namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class FakeObjectCreator
    {
        private static readonly ILogger logger = Log.GetLogger<FakeObjectCreator>();

        private IProxyGenerator2 proxyGenerator;

        public FakeObjectCreator(IProxyGenerator2 proxyGenerator)
        {
            logger.Debug("Creating new instance.");

            this.proxyGenerator = proxyGenerator;
        }

        internal object CreateFake(Type type, FakeOptions fakeOptions, IDummyValueCreationSession session, bool throwOnFailure)
        {
            logger.Debug("Creating fake of type {0}.", type);

            ProxyGeneratorResult result = new ProxyGeneratorResult("foo");

            foreach (var constructor in this.GetCandidateConstructorArguments(type, fakeOptions.ArgumentsForConstructor, session))
            {
                logger.Debug(() =>
                {
                    return string.Format("Trying to use constructor for {0} passing \"{1}\" as arguments.",
                        type, constructor.ResolvedArguments == null ? "<NULL>" : constructor.ResolvedArguments.ToCollectionString(x => x.ToString(), ", "));
                });

                result = this.proxyGenerator.GenerateProxy(type, fakeOptions.AdditionalInterfacesToImplement, constructor.ResolvedArguments);
            }

            if (throwOnFailure && !result.ProxyWasSuccessfullyGenerated)
            {
                throw new FakeCreationException();
            }

            return result.GeneratedProxy;
        }

        private IEnumerable<ConstructorArgumentsInfo> GetCandidateConstructorArguments(Type typeOfFake, IEnumerable<object> specifiedArgumentsForConstructor, IDummyValueCreationSession session)
        {
            if (specifiedArgumentsForConstructor != null)
            {
                logger.Debug("Using specified arguments as arguments for constructor.");

                return new[] 
		                {
		                    new ConstructorArgumentsInfo
		                    {
		                        ResolvedArguments = specifiedArgumentsForConstructor
		                    }
		                };
            }

            if (typeOfFake.IsInterface)
            {
                logger.Debug("Using null as arguments for constructor for interface {0}.", typeOfFake);

                return new[]
		                {
		                    new ConstructorArgumentsInfo
		                    {
		                        ResolvedArguments = null
		                    }
		                };
            }

            logger.Debug("Resolving constructors to try to use for proxy.");

            return
                from constructor in typeOfFake.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                select new ConstructorArgumentsInfo
                {
                    ResolvedArguments = from argument in constructor.GetParameters()
                                        let resolved = this.ResolveArgument(argument.ParameterType, session)
                                        where resolved.First == true
                                        select resolved.Second
                };
        }

        private Tuple<bool, object> ResolveArgument(Type typeOfArgument, IDummyValueCreationSession session)
        {
            logger.Debug("Trying to resolve {0} argument.", typeOfArgument);

            object result;
            return new Tuple<bool, object>(session.TryResolveDummyValue(typeOfArgument, out result), result);
        }

        private class ConstructorArgumentsInfo
        {
            public IEnumerable<object> ResolvedArguments { get; set; }
        }
    }
}
