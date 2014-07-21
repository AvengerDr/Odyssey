﻿using System.Linq;
using Odyssey.Engine;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using Odyssey.Talos.Nodes;
using System.Collections.Generic;

namespace Odyssey.Talos.Systems
{
    public abstract class LightSystem : UpdateableSystemBase
    {
        readonly LightCollection lightNodes;

        protected LightCollection LightNodes { get { return lightNodes; } }

        protected LightSystem(Selector selector) : base(selector)
        {
            lightNodes = new LightCollection();
        }

        protected void RemoveEntity(IEntity entity)
        {
            var node = (from kvp in lightNodes where kvp.Value.EntityId == entity.Id select kvp.Value).First();
            lightNodes.Remove(node.Id);
            Messenger.Send(new LightMessage(entity, node, ChangeType.Removed));
        }

        public override void Start()
        {
            Messenger.Register<ContentLoadedMessage<ShaderComponent>>(this);
            Messenger.Register<EntityChangeMessage>(this);
            Services.AddService(typeof(ILightService), lightNodes);
        }

        public override void Stop()
        {
            Messenger.Unregister<ContentLoadedMessage<ShaderComponent>>(this);
            Messenger.Unregister<EntityChangeMessage>(this);
        }

    }
}
