namespace Odyssey.Talos.Maps
{
    public class EntityComponentChangedEventArgs : EntityEventArgs
    {
        public IComponent Component { get; private set; }

        public EntityComponentChangedEventArgs(Entity source, IComponent component)
            : base(source)
        {
            Component = component;
        }
    }
}
