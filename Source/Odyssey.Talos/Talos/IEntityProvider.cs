using System.Collections.Generic;

namespace Odyssey.Talos
{
    public interface IEntityProvider
    {
        IEnumerable<Entity> Entities {get;}
        Entity SelectEntity(long id);
    }
}
