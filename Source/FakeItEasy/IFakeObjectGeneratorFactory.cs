namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Responsible for creating fake object generator commands.
    /// </summary>
    internal interface IFakeObjectGeneratorFactory
    {
        /// <summary>
        /// Creates a fake object generator command.
        /// </summary>
        /// <param name="typeOfFake">The type of fake to generate.</param>
        /// <param name="argumentsForConstructor">Of the faked type if any. Specify null when no arguments are provided.</param>
        /// <param name="allowNonProxiedFakes">If set to <c>true</c> fakes resolved from the IFakeObjectContainer are allowed.</param>
        /// <returns>The created command.</returns>
        IFakeObjectGenerator CreateGenerationCommand(Type typeOfFake, IEnumerable<object> argumentsForConstructor, bool allowNonProxiedFakes);
    }
}
