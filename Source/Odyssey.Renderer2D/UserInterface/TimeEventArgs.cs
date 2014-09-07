using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Engine;

namespace Odyssey.UserInterface
{
    public class TimeEventArgs : EventArgs
    {
        public ITimeService Time { get; private set; }

        public TimeEventArgs(ITimeService time)
        {
            Time = time;
        }
    }
}
