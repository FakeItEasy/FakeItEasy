namespace FakeItEasy.Tests.Core
{
    using NUnit.Framework;
    using FakeItEasy.Core;
    using System;

    [TestFixture]
    public class DefaultReturnValueRuleTests
        : ConfigurableServiceLocatorTestBase
    {
        private IFakeObjectGenerator generatorCommand;
        private IFakeObjectGeneratorFactory generatorCommandFactory;

        protected override void OnSetUp()
        {
            this.generatorCommand = A.Fake<IFakeObjectGenerator>();
            this.generatorCommandFactory = A.Fake<IFakeObjectGeneratorFactory>();

            A.CallTo(() => this.generatorCommand.GenerateFakeObject()).Returns(false);

            A.CallTo(() => this.generatorCommandFactory.CreateGenerationCommand(null, null, true))
                .WithAnyArguments()
                .Returns(this.generatorCommand);
        }

        private DefaultReturnValueRule CreateRule()
        {
            return new DefaultReturnValueRule();
        }

        [Test]
        public void Apply_should_set_return_value_to_value_from_generator_command_when_generator_command_returns_true()
        {
            // Arrange
            var rule = this.CreateRule();
            var call = A.Fake<IWritableFakeObjectCall>();

            object generatedFake = A.Fake<IFoo>();
            A.CallTo(() => this.generatorCommand.GeneratedFake).Returns(generatedFake);
            A.CallTo(() => this.generatorCommand.GenerateFakeObject()).Returns(true);

            // Act
            using (Fake.CreateScope())
            {
                this.StubResolve<IFakeObjectGeneratorFactory>(this.generatorCommandFactory);
                rule.Apply(call);    
            }
            
            // Assert
            A.CallTo(() => call.SetReturnValue(generatedFake)).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void Apply_should_call_factory_with_correct_parameters()
        {
            // Arrange
            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();
            A.CallTo(() => call.Method).Returns(typeof(IFoo).GetMethod("Baz", new Type[] { }));

            // Act
            using (Fake.CreateScope())
            {
                this.StubResolve<IFakeObjectGeneratorFactory>(this.generatorCommandFactory);
                rule.Apply(call);
            }


            // Assert
            A.CallTo(() => this.generatorCommandFactory.CreateGenerationCommand(typeof(int), null, true))
                .MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void Apply_should_set_return_value_to_default_value_when_generator_command_returns_false()
        {
            // Arrange
            var rule = this.CreateRule();
            var call = A.Fake<IWritableFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(typeof(IFoo).GetMethod("Baz", new Type[] { }));
            A.CallTo(() => this.generatorCommand.GenerateFakeObject()).Returns(false);

            // Act
            using (Fake.CreateScope())
            {
                this.StubResolve<IFakeObjectGeneratorFactory>(this.generatorCommandFactory);
                rule.Apply(call);
            }

            // Assert
            A.CallTo(() => call.SetReturnValue(0)).MustHaveHappened(Repeated.Once);
        }

        
    }
}
