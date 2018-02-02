namespace FakeItEasy.Tests.Configuration
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// Tests that apply to all implementations of <see cref="IAssertConfiguration"/>.
    /// </summary>
    public class AssertConfigurationTests
    {
        public interface IFoo
        {
            void VoidMethod();

            int WriteableProperty { get;  set; }
        }

        public static IEnumerable<object[]> AllKindsOfConfigurations()
        {
            var fake = A.Fake<IFoo>();
            return TestCases.FromObject(
                A.CallTo(() => fake.VoidMethod()),
                A.CallTo(fake),
                A.CallTo(fake).WithNonVoidReturnType(),
                A.CallToSet(() => fake.WriteableProperty),
                A.CallToSet(() => fake.WriteableProperty).To(7));
        }

        [Theory]
        [MemberData(nameof(AllKindsOfConfigurations))]
        public void MustHaveHappenedWithRepeated_should_be_null_guarded(IAssertConfiguration assertConfiguration)
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => assertConfiguration.MustHaveHappened(A.Dummy<Repeated>());
            call.Should().BeNullGuarded();
        }

        [Theory]
        [MemberData(nameof(AllKindsOfConfigurations))]
        public void MustHaveHappenedWithIntAndTimes_should_be_null_guarded(IAssertConfiguration assertConfiguration)
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => assertConfiguration.MustHaveHappened(A.Dummy<int>(), Times.Exactly);
            call.Should().BeNullGuarded();
        }

        [Theory]
        [MemberData(nameof(AllKindsOfConfigurations))]
        public void MustHaveHappenedANumberOfTimesMatching_should_be_null_guarded(IAssertConfiguration assertConfiguration)
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => assertConfiguration.MustHaveHappenedANumberOfTimesMatching(n => true);
            call.Should().BeNullGuarded();
        }
    }
}
