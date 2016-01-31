namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Indicates precedence between otherwise indistinguishable options, for example when deciding
    /// which <see cref="IDummyFactory"/> should be used to create a Dummy of a given type.
    /// In making such decisions, the higher-valued <c>Priority</c> will be used.
    /// </summary>
    public struct Priority : IComparable<Priority>, IEquatable<Priority>
    {
        private readonly short value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Priority"/> struct.
        /// </summary>
        /// <param name="value">The value of the <c>Priority</c>.</param>
        /// <returns>A new <c>Priority</c> with the specified value.</returns>
        public Priority(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Priority"/> struct.
        /// </summary>
        /// <param name="value">The value of the <c>Priority</c>.</param>
        /// <returns>A new <c>Priority</c> with the specified value.</returns>
        /// <remarks>
        /// This constructor is provided to allow internal FakeItEasy classes
        /// to construct negative-valued Priorities, while clients of FakeItEasy are
        /// restricted to nonnegative values.
        /// </remarks>
        internal Priority(short value)
        {
            this.value = value;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> is less than that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is less than that of <c>right</c>.</returns>
        public static bool operator <(Priority left, Priority right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> is greater than that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is greater than that of <c>right</c>.</returns>
        public static bool operator >(Priority left, Priority right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> is less than or equal to that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is less than or equal to that of <c>right</c>.</returns>
        public static bool operator <=(Priority left, Priority right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> is greater than or equal to that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is greater than or equal to that of <c>right</c>.</returns>
        public static bool operator >=(Priority left, Priority right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> is equal to that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is equal to that of <c>right</c>.</returns>
        public static bool operator ==(Priority left, Priority right)
        {
            return left.CompareTo(right) == 0;
        }

        /// <summary>
        /// Compares <see paramref="left"/> and <see paramref="right"/>, returning <c>true</c> if
        /// and only if the value of <c>left</c> not equal to that of <c>right</c>.
        /// </summary>
        /// <param name="left">One priority to compare.</param>
        /// <param name="right">The other priority to compare.</param>
        /// <returns><c>true</c> if and only if the value of <c>left</c> is not equal to that of <c>right</c>.</returns>
        public static bool operator !=(Priority left, Priority right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects
        /// being compared. The return value has these meanings:
        /// <list type="bullet">
        ///   <item>less than zero: this instance precedes <paramref name="other"/> in the sort order</item>
        ///   <item>zero: this instance occurs in the same position in the sort order as <paramref name="other"/></item>
        ///   <item>greater than zero: this instance follows <paramref name="other"/> in the sort order</item>
        /// </list>
        /// </returns>
        /// <param name="other">An object to compare with this instance.</param>
        public int CompareTo(Priority other)
        {
            return this.value.CompareTo(other.value);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns> True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Priority other)
        {
            return this.CompareTo(other) == 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object.
        /// </summary>
        /// <returns> True if the current object is equal to the <paramref name="obj"/> parameter; otherwise, false.</returns>
        /// <param name="obj">An object to compare with this object.</param>
        public override bool Equals(object obj)
        {
            return (obj is Priority) && this.Equals((Priority)obj);
        }

        /// <summary>
        /// Gets a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// Returns a textual description of this <see cref="Priority"/>.
        /// </summary>
        /// <returns>A textual description of this <see cref="Priority"/>, indicating its value.</returns>
        public override string ToString()
        {
            return "Priority<" + this.value + ">";
        }
    }
}
