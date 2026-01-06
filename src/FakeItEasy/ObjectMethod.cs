namespace FakeItEasy;

/// <summary>
/// Convenient identifiers for the <see cref="object"/> method a particular method might correspond to.
/// </summary>
internal enum ObjectMethod
{
    /// <summary>
    /// Corresponds to no object method.
    /// </summary>
    None,

    /// <summary>
    /// Corresponds to <see cref="object.Equals(object)"/>.
    /// </summary>
    EqualsMethod,

    /// <summary>
    /// Corresponds to <see cref="object.ToString"/>.
    /// </summary>
    ToStringMethod,

    /// <summary>
    /// Corresponds to <see cref="object.GetHashCode"/>.
    /// </summary>
    GetHashCodeMethod,
}
