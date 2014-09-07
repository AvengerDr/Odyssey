using System;
using System.Diagnostics.Contracts;
using Odyssey.Animations;
using Odyssey.Content;

namespace Odyssey.Talos.Components
{
    public class AnimationComponent : Component, IInitializable
    {
        public AnimationComponent() : base(ComponentTypeManager.GetType(typeof (AnimationComponent)))
        {
            Controller = new AnimationController();
        }

        public AnimationController Controller { get; set; }

        public bool IsInited
        {
            get; private set;
        }

        public void Initialize()
        {
            Contract.Requires<InvalidOperationException>(Controller != null, "Controller cannot be null");
            Controller.Initialize();
            IsInited = true;
        }
    }
}
