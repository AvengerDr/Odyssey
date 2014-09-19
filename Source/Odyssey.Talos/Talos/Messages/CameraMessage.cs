using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Engine;

namespace Odyssey.Talos.Messages
{
    public class CameraMessage : EntityChangeMessage
    {
        public ICamera Camera { get; private set; }

        public CameraMessage(Entity source, ICamera camera, UpdateType action, bool isSynchronous = false) : base(source, action, isSynchronous)
        {
            Camera = camera;
        }
    }
}
