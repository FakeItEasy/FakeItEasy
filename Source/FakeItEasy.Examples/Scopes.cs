namespace FakeItEasy.Examples
{
    using System;
    using System.Linq;
    using FakeItEasy.Examples.ExampleObjects;

    public class Scopes
    {
        public void Getting_calls_in_scope()
        {
            var factory = A.Fake<IWidgetFactory>();

            A.CallTo(() => factory.Create()).Returns(A.Fake<IWidget>());

            var widget = factory.Create();

            // This will succeed since there was a call to the create method
            // in the current scope.
            A.CallTo(() => factory.Create()).MustHaveHappened();

            using (Fake.CreateScope())
            {
                // This will produce 0, no calls were made to the create method 
                // in the current scope.
                Fake.GetCalls(factory).Matching<IWidgetFactory>(x => x.Create()).Count();

                // This will throw:
                A.CallTo(() => factory.Create()).MustHaveHappened();
            }

            using (Fake.CreateScope())
            {
                factory.Create();
            }

            // This will now return 2 since there was one call in the outer scope
            // and another one in the inner scope.
            Fake.GetCalls(factory).Matching<IWidgetFactory>(x => x.Create()).Count();
        }

        public void Configuring_calls_in_scope()
        {
            var factory = A.Fake<IWidgetFactory>();

            A.CallTo(() => factory.Create()).Returns(A.Fake<IWidget>());

            using (Fake.CreateScope())
            {
                A.CallTo(() => factory.Create()).Throws(new Exception());

                // This will throw since the method was configured in this scope.
                factory.Create();
            }

            // This will return a fake widget as configured before entering the scope.
            var widget = factory.Create();
        }

        public void Nesting_scopes()
        {
            // Scopes can be nested however deep you'd like:
            var factory = new Fake<IWidgetFactory>();

            using (Fake.CreateScope())
            {
                factory.AnyCall().Throws(new Exception());
                
                using (Fake.CreateScope())
                { 
                    // Will throw since it's configured in outer scope.
                    factory.FakedObject.Create();

                    factory.AnyCall().DoesNothing();

                    // Now will not throw since it's overridden in the inner scope.
                    factory.FakedObject.Create();
                }

                // Will throw  since it's configured in this scope.
                factory.FakedObject.Create();
            }

            // Returns null since it's not configured.
            factory.FakedObject.Create();
        }
    }
}
