namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Foo
    {
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Required for testing.")]
        public IServiceProvider ServiceProvider;

        public Foo(IServiceProvider provider)
        {
            this.ServiceProvider = provider;
        }

        public Foo()
        {
        }

        public virtual string VirtualProperty { get; set; }

        internal virtual IFoo InternalVirtualFakeableProperty { get; set; }

        public virtual void Baz()
        {
            throw new NotImplementedException();
        }

        public virtual string Bar()
        {
            throw new NotImplementedException();
        }

        public virtual string Bar(string baz, int bazz)
        {
            throw new NotImplementedException();
        }
    }
}
