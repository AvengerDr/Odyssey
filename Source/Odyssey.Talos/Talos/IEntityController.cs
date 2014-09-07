using Odyssey.Engine;

namespace Odyssey.Talos
{
    public interface IEntityController
    {
        void BindToEntity(Entity source);
        void Update(ITimeService time);
    }
}