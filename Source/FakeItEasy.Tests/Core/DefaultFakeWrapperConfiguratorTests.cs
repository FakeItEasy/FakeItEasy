using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FakeItEasy.Tests.Core
{
    class DefaultFakeWrapperConfiguratorTests
    {
        //[Test]
        //public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        //{
        //    var wrapped = A.Fake<IFoo>();
        //    var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

        //    A.CallTo(() => wrapped.Biz()).Returns("wrapped");
        //    A.CallTo(() => wrapper.Biz()).Returns("wrapper");

        //    Assert.That(wrapper.Biz(), Is.EqualTo("wrapper"));
        //}

        //[Test]
        //public void Fake_with_wrapped_instance_should_add_WrappedFakeObjectRule_to_fake_object()
        //{
        //    var wrapped = A.Fake<IFoo>();

        //    var foo = this.CreateFakeObject<IFoo>();
        //    A.CallTo(() => ((IFoo)foo.Object).ToString()).Returns("Tjena");

        //    A.CallTo(() => this.factory.CreateFake(A<Type>.Ignored, A<IEnumerable<object>>.Ignored.Argument, A<bool>.Ignored)).Returns(foo.Object);

        //    this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped));

        //    Assert.That(foo.Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
        //}

        //[Test]
        //public void Fake_with_wrapped_instance_and_recorder_should_add_SelfInitializationRule_to_fake_object()
        //{
        //    var recorder = A.Fake<ISelfInitializingFakeRecorder>();
        //    var wrapped = A.Fake<IFoo>();

        //    var wrapper = this.fakeObjectBuilder.GenerateFake<IFoo>(x => x.Wrapping(wrapped).RecordedBy(recorder));
        //    var fake = Fake.GetFakeObject(wrapper);

        //    Assert.That(fake.Rules.First(), Is.InstanceOf<SelfInitializationRule>());
        //}
    }
}
