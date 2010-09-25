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

        [SetUp]
        public void SetUp()
        {
            this.thrower = new DefaultExceptionThrower();
        }

        [Test]
        public void Should_throw_correct_exception_when_arguments_for_constructor_are_specified()
        {
            // Arrange
            
            // Act, Assert
            var ex = Assert.Throws<FakeCreationException>(() =>
                this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeof(string), @"a reason
that spans a couple of lines."));

            Assert.That(ex.Message, Is.EqualTo(
@"
  Failed to create fake of type ""System.String"" with the specified arguments for the constructor:
    a reason
    that spans a couple of lines.

"));
        }

        private object[] resolvedConstructorsTestCases = TestCases.Create
            (
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
                                    ResolvedValue = "",
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

"
                }
            ).AsTestCaseSource();
        
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
