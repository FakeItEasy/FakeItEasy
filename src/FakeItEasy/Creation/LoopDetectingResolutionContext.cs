namespace FakeItEasy.Creation;

using System;
using System.Collections.Generic;

internal class LoopDetectingResolutionContext
{
    private readonly HashSet<Type> typesCurrentlyBeingResolved = new HashSet<Type>();

    /// <summary>
    /// Indicate that we're trying to resolve a fake or dummy <paramref name="type"/>.
    /// Call <see cref="EndResolve"/> once the attempt to resolve the type is complete,
    /// so subsequent calls to this method will not detect a loop.
    /// </summary>
    /// <param name="type">The type that we're trying to resolve.</param>
    /// <returns>
    /// <c>true</c> if it's safe to resolve a <c>type</c>, or <c>false</c> if the action
    /// indicates that the type resolution system is in a loop.
    /// </returns>
    public bool TryBeginToResolve(Type type) => this.typesCurrentlyBeingResolved.Add(type);

    /// <summary>
    /// Indicate the end of an attempt to resolve a <paramref name="type"/> object.
    /// Subsequent calls to <see cref="TryBeginToResolve"/> will now pass.
    /// </summary>
    /// <param name="type">The type that was resolved.</param>
    public void EndResolve(Type type) => this.typesCurrentlyBeingResolved.Remove(type);
}
