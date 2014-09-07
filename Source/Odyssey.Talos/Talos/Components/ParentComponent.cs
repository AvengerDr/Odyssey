using SharpDX;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Components
{
    [YamlTag("Parent")]
    public class ParentComponent:Component
    {
        public Entity Parent { get; set; }

        public ParentComponent() : base(ComponentTypeManager.GetType<ParentComponent>())
        {
        }
    }
}
