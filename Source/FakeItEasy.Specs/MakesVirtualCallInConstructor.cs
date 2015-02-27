namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;

    public class MakesVirtualCallInConstructor
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
        public MakesVirtualCallInConstructor()
        {
            this.VirtualMethodValueDuringConstructorCall = this.VirtualMethod("call in constructor");
        }

        public string VirtualMethodValueDuringConstructorCall { get; private set; }

        public virtual string VirtualMethod(string parameter)
        {
            return "implementation value";
        }
    }
}