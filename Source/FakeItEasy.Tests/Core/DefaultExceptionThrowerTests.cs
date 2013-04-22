namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    internal class DefaultExceptionThrowerTests
    {
        private DefaultExceptionThrower thrower;

        private object[] resolvedConstructorsTestCases = TestCases.Create(
            new
            {
                TypeOfFake = typeof(string),
                ReasonForFailureOfDefault = "reason",
                ResolvedConstructors = new ResolvedConstructor[] { },
                ExpectedMessage = @"
  Failed to create fake of type ""System.String"".

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason

  If either the type or constructor is internal, try adding the following attribute to the assembly:
    [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"")]

"
            },
            new
            {
                TypeOfFake = typeof(int),
                ReasonForFailureOfDefault = "reason\r\non two lines",
                ResolvedConstructors = new ResolvedConstructor[] 
                {
                    new ResolvedConstructor
                    {
                        Arguments = new[]
                        {
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(int),
                                WasResolved = false
                            },
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(string),
                                WasResolved = true
                            }
                        }
                    },
                    new ResolvedConstructor
                    {
                        Arguments = new[]
                        {
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(object),
                                WasResolved = true
                            },
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(DateTime),
                                WasResolved = false
                            }
                        }
                    }
                },
                ExpectedMessage = @"
  Failed to create fake of type ""System.Int32"".

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason
      on two lines
    The following constructors were not tried:
      (*System.Int32, System.String)
      (System.Object, *System.DateTime)

      Types marked with * could not be resolved, register them in the current
      IFakeObjectContainer to enable these constructors.

  If either the type or constructor is internal, try adding the following attribute to the assembly:
    [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"")]

"
            },
            new
            {
                TypeOfFake = typeof(int),
                ReasonForFailureOfDefault = "reason\r\non two lines",
                ResolvedConstructors = new ResolvedConstructor[] 
                {
                    new ResolvedConstructor
                    {
                        Arguments = new[]
                        {
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(int),
                                WasResolved = false
                            },
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(string),
                                WasResolved = true
                            }
                        }
                    },
                    new ResolvedConstructor
                    {
                        ReasonForFailure = "message from proxy generator",
                        Arguments = new[]
                        {
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(string),
                                ResolvedValue = string.Empty,
                                WasResolved = true
                            }
                        }
                    },
                    new ResolvedConstructor
                    {
                        Arguments = new[]
                        {
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(object),
                                WasResolved = true
                            },
                            new ResolvedArgument
                            {
                                ArgumentType = typeof(DateTime),
                                WasResolved = false
                            }
                        }
                    }
                },
                ExpectedMessage = @"
  Failed to create fake of type ""System.Int32"".

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason
      on two lines
    Constructor with signature (System.String) failed:
      message from proxy generator
    The following constructors were not tried:
      (*System.Int32, System.String)
      (System.Object, *System.DateTime)

      Types marked with * could not be resolved, register them in the current
      IFakeObjectContainer to enable these constructors.

  If either the type or constructor is internal, try adding the following attribute to the assembly:
    [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"")]

"
            }).AsTestCaseSource();

        [SetUp]
        public void SetUp()
        {
            this.thrower = new DefaultExceptionThrower();
        }

        [Test]
        public void Should_throw_correct_exception_when_arguments_for_constructor_are_specified()
        {
            // Arrange
            var reason =
@"a reason
that spans a couple of lines.";

            // Act, Assert
            var ex = Assert.Throws<FakeCreationException>(() => this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeof(string), reason));
            var expectedMessage =
@"
  Failed to create fake of type ""System.String"" with the specified arguments for the constructor:
    a reason
    that spans a couple of lines.

  If either the type or constructor is internal, try adding the following attribute to the assembly:
    [assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"")]

";
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [TestCaseSource("resolvedConstructorsTestCases")]
        public void Should_throw_correct_exception_when_resolved_constructors_are_used(
            Type typeOfFake, string reasonForFailureOfUnspecifiedConstructor, IEnumerable<ResolvedConstructor> resolvedConstructors, string expectedMessage)
        {
            // Arrange

            // Act
            var ex = Assert.Throws<FakeCreationException>(() =>
                this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(
                    typeOfFake, reasonForFailureOfUnspecifiedConstructor, resolvedConstructors));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}
