
namespace Odyssey.Talos.Messages
{
    public abstract class EntityMessage : Message
    {
        public Entity Source { get; private set; }

        protected EntityMessage(Entity source, bool isSynchronous = false)
            : base(isSynchronous)
        {
            Source = source;
        }
    }

}
