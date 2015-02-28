namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class MakesVirtualCallInConstructor
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Required for testing.")]
        public MakesVirtualCallInConstructor()
        {
            try
            {
                this.VirtualMethodValueDuringConstructorCall = this.VirtualMethod("call in constructor");
            }
            catch (Exception e)
            {
                this.ExceptionFromVirtualMethodCallInConstructor = e;
            }
        }

        public Exception ExceptionFromVirtualMethodCallInConstructor { get; private set; }

        public string VirtualMethodValueDuringConstructorCall { get; private set; }

        public virtual string VirtualMethod(string parameter)
        {
            return "implementation value";
        }
    }
}