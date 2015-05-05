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

        public string ConstructorArgument1 { get; private set; }

        public int ConstructorArgument2 { get; private set; }

        public Exception ExceptionFromVirtualMethodCallInConstructor { get; private set; }

        public string VirtualMethodValueDuringConstructorCall { get; private set; }

        public virtual string VirtualMethod(string parameter)
        {
            return "implementation value";
        }
    }
}