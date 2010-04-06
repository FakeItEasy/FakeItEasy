//namespace FakeItEasy.Tests.Core
//{
//    using NUnit.Framework;
//    using FakeItEasy.Core;
//    using System;
//using FakeItEasy.Core.Creation;
//    using FakeItEasy.Tests.TestHelpers;

//    [TestFixture]
//    public class DefaultReturnValueRuleTests
//        : ConfigurableServiceLocatorTestBase
//    {
//        private IFakeAndDummyManager fakeManager;

//        protected override void OnSetUp()
//        {
//            this.fakeManager = A.Fake<IFakeAndDummyManager>();
//        }

//        private DefaultReturnValueRule CreateRule()
//        {
//            return new DefaultReturnValueRule();
//        }

//        [Test]
//        public void Apply_should_set_return_value_to_value_from_generator_command_when_generator_command_returns_true()
//        {
//            // Arrange
//            var rule = this.CreateRule();
//            var call = A.Fake<IWritableFakeObjectCall>();

//            object generatedFake = A.Fake<IFoo>();
//            A.CallTo(() => this.fakeManager.TryCreateDummy(typeof(IFoo), out Null<object>.Out))
//                .Returns(true)
//                .AssignsOutAndRefParameters(generatedFake);

//            // Act
//            using (Fake.CreateScope())
//            {
//                this.StubResolve<IFakeAndDummyManager>(this.fakeManager);
//                rule.Apply(call);    
//            }
            
//            // Assert
//            A.CallTo(() => call.SetReturnValue(generatedFake)).MustHaveHappened();
//        }

//        [Test]
//        public void Apply_should_call_factory_with_correct_parameters()
//        {
//            // Arrange
//            var rule = this.CreateRule();

//            var call = A.Fake<IWritableFakeObjectCall>();
//            A.CallTo(() => call.Method).Returns(typeof(IFoo).GetMethod("Baz", new Type[] { }));

//            // Act
//            using (Fake.CreateScope())
//            {
//                this.StubResolve<IFakeAndDummyManager>(this.fakeManager);
//                rule.Apply(call);
//            }


//            // Assert
//            A.CallTo(() => this.fakeManager.TryCreateDummy(typeof(int), out Null<object>.Out)).MustHaveHappened();
//        }

//        [Test]
//        public void Apply_should_set_return_value_to_default_value_when_generator_command_returns_false()
//        {
//            // Arrange
//            var rule = this.CreateRule();
//            var call = A.Fake<IWritableFakeObjectCall>();

//            A.CallTo(() => call.Method).Returns(typeof(IFoo).GetMethod("Baz", new Type[] { }));
//            A.CallTo(() => this.fakeManager.TryCreateDummy(A<Type>.Ignored, out Null<object>.Out)).Returns(false);

//            // Act
//            using (Fake.CreateScope())
//            {
//                this.StubResolve<IFakeAndDummyManager>(this.fakeManager);
//                rule.Apply(call);
//            }

//            // Assert
//            A.CallTo(() => call.SetReturnValue(0)).MustHaveHappened();
//        }

        
//    }
//}
