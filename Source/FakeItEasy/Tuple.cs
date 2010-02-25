namespace FakeItEasy
{
    /// <summary>
    /// Represents a tuple of two values.
    /// </summary>
    /// <typeparam name="TFirst">The first value.</typeparam>
    /// <typeparam name="TSecond">The second value.</typeparam>
    internal class Tuple<TFirst, TSecond>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;TFirst, TSecond&gt;"/> class.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        public Tuple(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <value>The first.</value>
        public TFirst First { get; private set; }

        /// <summary>
        /// Gets the second value.
        /// </summary>
        /// <value>The second.</value>
        public TSecond Second { get; private set; }
    }
}
