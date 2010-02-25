using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Windows.Forms;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Debugging;

[assembly: DebuggerVisualizer(
    typeof(FakeObjectVisualizer), 
    typeof(VisualizerObjectSource),
    Target = typeof(FakeItEasy.Api.IFakedProxy), 
    Description = "Fake object visualizer")]

namespace FakeItEasy.Debugging
{
    public class FakeObjectProvider
        : IVisualizerObjectProvider
    {
        public System.IO.Stream GetData()
        {
            throw new NotImplementedException();
        }

        public object GetObject()
        {
            throw new NotImplementedException();
        }

        public bool IsObjectReplaceable
        {
            get { return false; }
        }

        public void ReplaceData(System.IO.Stream newObjectData)
        {
            throw new NotImplementedException();
        }

        public void ReplaceObject(object newObject)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream TransferData(System.IO.Stream outgoingData)
        {
            throw new NotImplementedException();
        }

        public object TransferObject(object outgoingObject)
        {
            throw new NotImplementedException();
        }
    }


    public class FakeObjectVisualizer
        : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var fakedObject = objectProvider.GetObject();
            var fakeObject = Fake.GetFakeObject(fakedObject);

            var message = new StringBuilder()
                .Append(fakedObject.GetType().Name)
                .AppendLine()
                .Append(string.Join(", ", fakeObject.Rules.Select(x => x.ToString()).ToArray()));

            MessageBox.Show(message.ToString());
        }
    }
}
