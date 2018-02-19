namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DefaultExceptionThrowerTests
    {
        public static IEnumerable<object> ResolvedConstructorsTestCases()
        {
            return TestCases.FromObject(
                new DefaultExceptionThrowerTestCase("only parameterless constructor")
                {
                    TypeOfFake = typeof(string),
                    ReasonForFailureOfDefaultConstructor = "reason",
                    ResolvedConstructors = Array.Empty<ResolvedConstructor>(),
                    ExpectedMessage = @"
  Failed to create fake of type System.String.

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason

"
                },
                new DefaultExceptionThrowerTestCase("multi-line reason for constructor failure")
                {
                    TypeOfFake = typeof(int),
                    ReasonForFailureOfDefaultConstructor = "reason\r\non two lines",
                    ResolvedConstructors = new[]
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
  Failed to create fake of type System.Int32.

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason
      on two lines
    The following constructors were not tried:
      (*System.Int32, System.String)
      (System.Object, *System.DateTime)

      Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

"
                },
                new DefaultExceptionThrowerTestCase("parameterful constructor failed")
                {
                    TypeOfFake = typeof(int),
                    ReasonForFailureOfDefaultConstructor = "reason\r\non two lines",
                    ResolvedConstructors = new[]
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
  Failed to create fake of type System.Int32.

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      reason
      on two lines
    Constructor with signature (System.String) failed:
      message from proxy generator
    The following constructors were not tried:
      (*System.Int32, System.String)
      (System.Object, *System.DateTime)

      Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.

"
                });
        }

        [Fact]
        public void Should_throw_correct_exception_when_arguments_for_constructor_are_specified()
        {
            // Arrange
            var reason =
@"a reason
that spans a couple of lines.";
            var thrower = new DefaultExceptionThrower();

            // Act
            var exception = Record.Exception(
                () => thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeof(string), reason));

            // Assert
            var expectedMessage =
@"
  Failed to create fake of type System.String with the specified arguments for the constructor:
    a reason
    that spans a couple of lines.
";

            exception.Should()
                .BeAnExceptionOfType<FakeCreationException>()
                .WithMessage(expectedMessage);
        }

        [Theory]
        [MemberData(nameof(ResolvedConstructorsTestCases))]
        public void Should_throw_correct_exception_when_resolved_constructors_are_used(DefaultExceptionThrowerTestCase testCase)
        {
            // Arrange
            var thrower = new DefaultExceptionThrower();

            // Act
            var exception = Record.Exception(
                () => thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(
                    testCase.TypeOfFake, testCase.ReasonForFailureOfDefaultConstructor, testCase.ResolvedConstructors));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<FakeCreationException>()
                .WithMessage(testCase.ExpectedMessage);
        }

        public class DefaultExceptionThrowerTestCase
        {
            private readonly string description;

            public DefaultExceptionThrowerTestCase(string description)
            {
                this.description = description;
            }

            internal Type TypeOfFake { get; set; }

            internal string ReasonForFailureOfDefaultConstructor { get; set; }

            internal ResolvedConstructor[] ResolvedConstructors { get; set; }

            internal string ExpectedMessage { get; set; }

            public override string ToString()
            {
                return this.description;
            }
        }
    }
}
