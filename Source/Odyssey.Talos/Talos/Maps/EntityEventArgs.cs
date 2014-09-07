using System;

namespace Odyssey.Talos.Maps
{
    public class EntityEventArgs : EventArgs
    {
        public Entity Source { get; private set; }

        public EntityEventArgs(Entity entity)
        {
            Source = entity;
        }
    }
}
