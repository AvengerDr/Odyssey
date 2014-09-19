namespace Odyssey.Talos.Messages
{
    public class EntityChangeMessage : EntityMessage
    {
        public UpdateType Action { get; private set; }

        public EntityChangeMessage(Entity source, UpdateType action, bool isSynchronous = false)
            : base(source, isSynchronous)
        {
            Action = action;
        }
    }
}
