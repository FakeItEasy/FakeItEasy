namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy.Core;

    /// <summary>
    /// Provides extension methods for writing object calls to output devices.
    /// </summary>
    public static class WriteExtensions
    {
        /// <summary>
        /// Writes the calls in the collection to the specified output writer.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        /// <param name="writer">The writer to write the calls to.</param>
        public static void Write<T>(this IEnumerable<T> calls, IOutputWriter writer) where T : IFakeObjectCall
        {
            Guard.AgainstNull(calls, "calls");
            Guard.AgainstNull(writer, "writer");

            var callWriter = ServiceLocator.Current.Resolve<CallWriter>();
            callWriter.WriteCalls(calls.Cast<IFakeObjectCall>(), writer);
        }

        /// <summary>
        /// Writes all calls in the collection to the console.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        public static void WriteToConsole<T>(this IEnumerable<T> calls) where T : IFakeObjectCall
        {
            calls.Write(new DefaultOutputWriter(Console.Write));
        }
    }
}