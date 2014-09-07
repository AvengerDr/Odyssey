namespace Odyssey.Talos.Messages
{
    public class EntityChangeMessage : EntityMessage
    {
        public ChangeType Action { get; private set; }

        public EntityChangeMessage(Entity source, ChangeType action, bool isSynchronous = false)
            : base(source, isSynchronous)
        {
            Action = action;
        }
    }
}
