using System;
using System.Reflection;
using FakeItEasy.Core;

namespace FakeItEasy.Tests
{
    /// <summary>
    /// A fake implementation of IFakeObjectCall, used for testing.
    /// </summary>
    public class FakeCall
        : IInterceptedFakeObjectCall, ICompletedFakeObjectCall
    {
        public FakeCall()
        {
            this.Arguments = ArgumentCollection.Empty;
        }

        public MethodInfo Method
        {
            get;
            set;
        }

        public ArgumentCollection Arguments
        {
            get;
            set;
        }


        public void SetReturnValue(object returnValue)
        {
            this.ReturnValue = returnValue;           
        }

        public static FakeCall Create<T>(string methodName, Type[] parameterTypes, object[] arguments) where T : class
        {
            var method = typeof(T).GetMethod(methodName, parameterTypes);
            
            return new FakeCall
            {
                Method = method,
                Arguments = new ArgumentCollection(arguments, method),
                FakedObject = A.Fake<T>()
            };
        }

        public static FakeCall Create<T>(string methodName) where T : class
        {
            return Create<T>(methodName, new Type[] { }, new object[] { });
        }

        public object ReturnValue
        {
            get;
            private set;
        }


        public object FakedObject
        {
            get;
            set;
        }

        public string Description
        {
            get { return this.ToString(); }
        }

        public ICompletedFakeObjectCall AsReadOnly()
        {
            return this;
        }


        public void CallBaseMethod()
        {
            
        }

        public void SetArgumentValue(int index, object value)
        {
            
        }

        public void DoNotRecordCall()
        {

        }
    }
}