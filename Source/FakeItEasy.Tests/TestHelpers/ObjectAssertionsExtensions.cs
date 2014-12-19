namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions.Primitives;
    using FluentAssertions.Specialized;

    /// <summary>
    /// Extends the capabilities of <see cref="ObjectAssertions"/>.
    /// </summary>
    public static class ObjectAssertionsExtensions
    {
        /// <summary>
        /// Verifies that the passed-in assertion refers to a <see cref="ReferenceTypeAssertions{TSubject, TAssertions}.Subject"/>
        /// that is a Fake.
        /// </summary>
        /// <param name="assertion">A FluentAssertions assertion that has been initiated on a subject.</param>
        public static void BeAFake(this ReferenceTypeAssertions<object, ObjectAssertions> assertion)
        {
            Guard.AgainstNull(assertion, "assertion");

            assertion
                .NotBeNull("because fakes aren't null").And
                .Match(subject => Fake.GetFakeManager(subject) != null, "fakes have a FakeManager");
        }

        /// <summary>
        /// Verifies that the passed-in assertion refers to a <see cref="ReferenceTypeAssertions{TSubject, TAssertions}.Subject"/>
        /// that is not null and matches the expected exception type.
        /// </summary>
        /// <typeparam name="TExpectedException">The expected exception type.</typeparam>
        /// <param name="assertion">A FluentAssertions assertion that has been initiated on a subject.</param>
        /// <returns>An <see cref="ExceptionAssertions{T}"/> object that can be further used to assert against the subject.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BeAn", Justification = "Refers to the two words 'be an'")]
        public static ExceptionAssertions<TExpectedException> BeAnExceptionOfType<TExpectedException>(this ReferenceTypeAssertions<object, ObjectAssertions> assertion) where TExpectedException : Exception
        {
            Guard.AgainstNull(assertion, "assertion");

            assertion
                .NotBeNull("because it must be a valid exception").And
                .BeOfType<TExpectedException>();

            var exception = (TExpectedException)assertion.Subject;
            return new MyExceptionAssertions<TExpectedException>(exception);
        }

        /// <summary>
        /// Verifies that the passed-in assertion refers to a <see cref="ReferenceTypeAssertions{TSubject, TAssertions}.Subject"/>
        /// that is not null and is assignable to the expected exception type.
        /// </summary>
        /// <typeparam name="TExpectedException">The expected exception type.</typeparam>
        /// <param name="assertion">A FluentAssertions assertion that has been initiated on a subject.</param>
        /// <returns>An <see cref="ExceptionAssertions{T}"/> object that can be further used to assert against the subject.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BeAn", Justification = "Refers to the two words 'be an'")]
        public static ExceptionAssertions<TExpectedException> BeAnExceptionAssignableTo<TExpectedException>(this ReferenceTypeAssertions<object, ObjectAssertions> assertion) where TExpectedException : Exception
        {
            Guard.AgainstNull(assertion, "assertion");

            assertion
                .NotBeNull("because it must be a valid exception").And
                .BeAssignableTo<TExpectedException>();

            var exception = (TExpectedException)assertion.Subject;
            return new MyExceptionAssertions<TExpectedException>(exception);
        }

        /// <summary>
        /// A convenient extension so we can access the <see cref="ExceptionAssertions{TException}"/> constructor.
        /// </summary>
        /// <typeparam name="TException">The type of exception to require.</typeparam>
        private class MyExceptionAssertions<TException> : ExceptionAssertions<TException> where TException : Exception
        {
            public MyExceptionAssertions(TException exception)
                : base(new[] { exception })
            {
            }
        }
    }
}