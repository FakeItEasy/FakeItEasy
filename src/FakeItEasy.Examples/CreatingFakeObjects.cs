namespace FakeItEasy.Examples
{
    using FakeItEasy.Examples.ExampleObjects;

    public class CreatingFakeObjects
    {
        public void Creating_an_interface_fake()
        {
            var foo = A.Fake<IWidgetFactory>();
        }

        public void Creating_a_fake_for_a_class_that_takes_constructor_arguments()
        {
            // Not that the arguments for the constructor is specified as
            // an expression rather than an object array and is "refactor friendly".
            var fake = A.Fake<ClassThatTakesConstructorArguments>(x => x.WithArgumentsForConstructor(() =>
                new ClassThatTakesConstructorArguments(A.Fake<IWidgetFactory>(), "name")));
        }

        public void Properties_of_types_that_can_be_faked_are_set_to_fake_objects_by_default()
        {
            var factory = A.Fake<IWidgetFactory>();

            // The following call is legal since by default property values are set
            // to fake objects.
            A.CallTo(() => factory.SubFactory.Create()).Returns(A.Fake<IWidget>());
        }

        public void Create_a_fake_object_that_has_methods_for_configuring_the_faked_instance()
        {
            var fake = new Fake<IWidgetFactory>();

            // Calls can be configured directly:
            fake.CallsTo(x => x.Create()).Returns(new Fake<IWidget>().FakedObject);

            // Getting the faked instance:
            var faked = fake.FakedObject;
        }
    }
}
