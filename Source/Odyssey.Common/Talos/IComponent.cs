using Odyssey.Content;

namespace Odyssey.Talos
{
    public interface IComponent : IResource
    {
        long KeyPart { get; }
        long Id { get; }
        bool Validate();
    }
}
