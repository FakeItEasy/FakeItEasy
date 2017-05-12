namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Creation;

    /// <summary>
    /// A base implementation for classes that can build options for fakes of type <typeparamref name="TFake"/>.
    /// </summary>
    /// <typeparam name="TFake">The type of fake.</typeparam>
    public abstract class FakeOptionsBuilder<TFake> : IFakeOptionsBuilder
    {
        /// <summary>
        /// Gets the priority of the options builder. When multiple builders that apply to
        /// the same type are registered, the one with the highest priority value is used.
        /// </summary>
        /// <remarks>The default implementation returns <see cref="FakeItEasy.Priority.Default"/>.</remarks>
        public virtual Priority Priority => Priority.Default;

        /// <summary>
        /// Whether or not this object can build options for a Fake of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of fake to build options for.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is <typeparamref name="TFake"/>.
        /// Otherwise <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Explicit implementation reduces the chance of misusing object. Explicit methods are superseded by BuildOptions(IFakeOptions<TFake>)")]
        bool IFakeOptionsBuilder.CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(TFake);
        }

        /// <summary>
        /// Manipulates <paramref name="options"/>, which will later be used to
        /// create a Fake.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="options">The fake options to manipulate.</param>
        /// <exception cref="InvalidOperationException">When <paramref name="typeOfFake"/> is not <typeparamref name="TFake"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Explicit implementation reduces the chance of misusing object. Explicit methods are superseded by BuildOptions(IFakeOptions<TFake>)")]
        void IFakeOptionsBuilder.BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            if (!((IFakeOptionsBuilder)this).CanBuildOptionsForFakeOfType(typeOfFake))
            {
                throw new InvalidOperationException(
                    $"Specified type {typeOfFake} is not valid. Only {typeof(TFake)} is allowed.");
            }

            this.BuildOptions((IFakeOptions<TFake>)options);
        }

        /// <summary>
        /// Manipulates <paramref name="options"/>, which will later be used to
        /// create a Fake.
        /// </summary>
        /// <param name="options">The fake options to manipulate.</param>
        protected abstract void BuildOptions(IFakeOptions<TFake> options);
    }
}
