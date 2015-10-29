namespace FakeItEasy
{
    using System;
    using Creation;

    /// <summary>
    /// A base implementation for classes that can build options for fakes of type <typeparamref name="TFake"/>.
    /// </summary>
    /// <typeparam name="TFake">The type of fake.</typeparam>
    public abstract class FakeOptionsBuilder<TFake> : IFakeOptionsBuilder
    {
        /// <summary>
        /// Gets the priority of the options builder. When multiple builders that apply to
        /// the same type are registered, the one with the highest priority is used.
        /// </summary>
        /// <remarks>Defaults to <c>0</c>. Negative values are reserved for use by FakeItEasy.</remarks>
        public virtual int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Whether or not this object can build options for a Fake of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of fake to build options for.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is <typeparamref name="TFake"/>.
        /// Otherwise <c>false</c>.
        /// </returns>
        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            return type == typeof(TFake);
        }

        /// <summary>
        /// Manipulates <paramref name="options"/>, which will later be used to 
        /// create a Fake.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="options">The fake options to manipulate.</param>
        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
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
