namespace FakeItEasy
{
    /// <summary>
    /// Represents a tuple of two values.
    /// </summary>
    /// <typeparam name="TFirst">The first value.</typeparam>
    /// <typeparam name="TSecond">The item2 value.</typeparam>
    internal class Tuple<TFirst, TSecond>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;TFirst, TSecond&gt;"/> class.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        public Tuple(TFirst item1, TSecond item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <value>The first.</value>
        public TFirst Item1 { get; private set; }

        /// <summary>
        /// Gets the item2 value.
        /// </summary>
        /// <value>The item2.</value>
        public TSecond Item2 { get; private set; }
    }
}
