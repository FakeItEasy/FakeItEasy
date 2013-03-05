namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Foo
    {
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Testing only.")]
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

        public virtual string Bar(string baz, int lorem)
        {
            throw new NotImplementedException();
        }
    }
}
