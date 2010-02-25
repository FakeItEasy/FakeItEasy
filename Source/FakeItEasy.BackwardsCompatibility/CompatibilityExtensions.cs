using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using System.Linq.Expressions;

namespace FakeItEasy
{
    public static class CompatibilityExtensions
    {
        /// <summary>
        /// Asserts right away that the configured must have happened at least once.
        /// </summary>
        public static void MustHaveHappened(this IAssertConfiguration asserts)
        {
            asserts.MustHaveHappened(repeat => repeat > 0);
        }

        public static void MustHaveHappened(this IAssertConfiguration configuration, Expression<Func<int, bool>> repeatPredicate)
        {
            configuration.Assert(Happened.ConvertFromExpression(repeatPredicate));
        }
    }
}
