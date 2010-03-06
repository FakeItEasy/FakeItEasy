namespace FakeItEasy.VisualBasic
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Configurations for visual basic.
    /// </summary>
    /// <typeparam name="TFake">The type of the configured faked object.</typeparam>
    public interface IVisualBasicConfiguration
            : IVoidConfiguration
    {
        /// <summary>
        /// Asserts that the specified call was made the number of times validated
        /// by the repeat predicate.
        /// </summary>
        /// <param name="repeatPredicate">A predicate expression that validates the number of times
        /// the specifed call was made.</param>
        /// <typeparam name="TFake">The type of faked object.</typeparam>
        void AssertWasCalled(Func<int, bool> repeatPredicate);
    }

    public interface IVisualBasicConfigurationWithArgumentValidation
        : IVisualBasicConfiguration, IArgumentValidationConfiguration<IVisualBasicConfiguration>
    { 
    
    }
}
