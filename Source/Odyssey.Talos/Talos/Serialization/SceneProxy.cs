using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Talos.Components;
using Odyssey.Talos.Systems;
using SharpYaml;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Serialization
{
    [YamlTag("Scene")]
    public class SceneProxy
    {
        //private readonly List<YamlEntity> entities;

        [YamlMember(0)]
        public List<ISystem> Systems { get; set; }

        [YamlMember(1)]
        public Dictionary<Entity, List<Component>> Entities { get; set; }

        //public IEnumerable<YamlEntity> Entities { get { return entities; } }
        private Dictionary<long, long> entityMap;

        public SceneProxy()
        {
            Entities = new Dictionary<Entity, List<Component>>();
            entityMap = new Dictionary<long, long>();
        }

        public SceneProxy(Scene scene) : this()
        {
            
            foreach (IEntity entity in scene.Entities)
            {
                var clonedEntity = new Entity(entity.Name);
                Entities.Add(clonedEntity, entity.Components.Cast<Component>().ToList());
                entityMap.Add(entity.Id, clonedEntity.Id);
            }
            FixParents();
            Systems = new List<ISystem>(scene.Systems);
        }

        void FixParents()
        {
            var parentComponents = Entities.SelectMany(kvp => kvp.Value).OfType<ParentComponent>();

            foreach (var cParent in parentComponents)
            {
                long newId = entityMap[cParent.Parent.Id];
                cParent.Parent = Entities.First(kvp => kvp.Key.Id == newId).Key;
            }

        }
    }
}
