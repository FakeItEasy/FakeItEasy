using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.ExtensionSyntax;

namespace FakeItEasy.Examples
{
    public interface IWidgetFactory
    {
        IWidget Create();
        IWidgetFactory SubFactory { get; set; }
    }

    

    public interface IWidget
    {
        string Name { get; set; }
        event EventHandler<WidgetEventArgs> WidgetBroke;
        void Repair();
    }

    [Serializable]
    public class WidgetEventArgs
        : EventArgs
    {
        public WidgetEventArgs(string widgetName)
        { 
            this.WidgetName = widgetName;
        }

        public string WidgetName { get; private set; }
    }
    
    public class ClassThatTakesConstructorArguments
    {
        public ClassThatTakesConstructorArguments(IWidgetFactory foo, string name)
        {

        }
    }
}
