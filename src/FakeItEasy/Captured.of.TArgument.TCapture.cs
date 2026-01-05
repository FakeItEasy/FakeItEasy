namespace FakeItEasy;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Holds argument values captured from calls to a faked member.
/// </summary>
/// <typeparam name="TArgument">The type of the argument.</typeparam>
/// <typeparam name="TCapture">The type of the value to capture from the argument.</typeparam>
public class Captured<TArgument, TCapture>
{
    private readonly List<TCapture> values = [];
    private readonly Func<TArgument, TCapture> freezer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Captured{TArgument, TCapture}"/> class.
    /// </summary>
    /// <param name="freezer">
    ///   Transforms incoming argument values before storing the result.
    ///   Useful when argument values may be mutated between calls and you want
    ///   to store a copy that will not be modified.
    /// </param>
    internal Captured(Func<TArgument, TCapture> freezer) => this.freezer = Guard.AgainstNull(freezer);

    /// <summary>
    /// Gets an argument constraint object that will be used to constrain this captured method call argument.
    /// </summary>
    public INegatableArgumentConstraintManager<TArgument> That =>
        ServiceLocator.Resolve<IArgumentConstraintManagerFactory>().Create<TArgument>().IsCapturedTo(this);

    /// <summary>
    /// Gets a constraint that considers any value of this captured argument as valid.
    /// </summary>
    /// <remarks>This is a shortcut for the "Ignored"-property.</remarks>
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1707:IdentifiersShouldNotContainUnderscores",
        Justification = "But it's kinda cool right?")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
#pragma warning disable SA1300 // Element must begin with upper-case letter
    public TArgument _ => this.Ignored;
#pragma warning restore SA1300 // Element must begin with upper-case letter

    /// <summary>
    /// Gets a constraint that considers any value of this captured argument as valid.
    /// </summary>
    public TArgument Ignored => this.That.Matches(x => true, x => x.Write(nameof(this.Ignored)));

    /// <summary>
    /// Gets the captured values.
    /// </summary>
    /// <remarks>
    /// Will be empty if no arguments were captured.
    /// </remarks>
    public IReadOnlyList<TCapture> Values => this.values;

    /// <summary>
    /// Gets the last captured value.
    /// </summary>
    /// <returns>
    /// The last captured value.
    /// </returns>
    /// <exception cref="ExpectationException">
    /// If no arguments were captured.
    /// </exception>
    public TCapture GetLastValue()
    {
        if (this.values.Count == 0)
        {
            throw new ExpectationException(ExceptionMessages.NoCapturedValues);
        }

        return this.values.Last();
    }

    /// <summary>
    /// Add a captured value.
    /// </summary>
    /// <param name="value">The new value to capture.</param>
    internal void Add(TArgument value) => this.values.Add(this.freezer(value));
}
