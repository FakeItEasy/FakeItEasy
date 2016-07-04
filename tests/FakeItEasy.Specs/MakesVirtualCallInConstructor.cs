namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class MakesVirtualCallInConstructor
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Required for testing.")]
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

        public MakesVirtualCallInConstructor(string argument1, int argument2)
        {
            this.ConstructorArgument1 = argument1;
            this.ConstructorArgument2 = argument2;
        }

        public string ConstructorArgument1 { get; }

        public int ConstructorArgument2 { get; }

        public Exception ExceptionFromVirtualMethodCallInConstructor { get; }

        public string VirtualMethodValueDuringConstructorCall { get; }

        public virtual string VirtualMethod(string parameter)
        {
            return "implementation value";
        }
    }
}
