using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.UserInterface.Controls
{
    public class TextEventArgs : EventArgs
    {
        public string OldValue { get; private set; }
        public string NewValue { get; private set; }

        public TextEventArgs(string newValue, string oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
