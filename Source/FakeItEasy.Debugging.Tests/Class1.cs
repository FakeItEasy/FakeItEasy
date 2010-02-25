using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NUnit.Framework;

namespace FakeItEasy.Debugging.Tests
{
    public class Class1
    {
        [Test]
        public void test_name()
        {

            object skdj = "dkfjk";

            // Arrange
            var foo = A.Fake<ISomething>();
            Configure.Fake(foo).CallsTo(x => x.Bar()).Throws(new Exception());
            var host = new VisualizerDevelopmentHost(foo, typeof(FakeObjectVisualizer));

            // Act
            host.ShowVisualizer();
            // Assert
            
        }

        public interface ISomething
        {
            void Bar();
            int Baz(string s);
        }
    }
}
