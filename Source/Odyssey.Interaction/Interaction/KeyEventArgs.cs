using System;

namespace Odyssey.Interaction
{
    public class KeyEventArgs : EventArgs
    {
        public bool Handled { get; internal set; }
        public Keys KeyData { get; private set; }

        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }
    }
}
