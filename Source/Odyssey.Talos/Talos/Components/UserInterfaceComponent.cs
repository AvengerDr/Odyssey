using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Odyssey.Content;
using Odyssey.Graphics;
using Odyssey.UserInterface;
using Odyssey.UserInterface.Controls;
using System;
using System.Diagnostics.Contracts;
using Odyssey.Utilities.Reflection;

namespace Odyssey.Talos.Components
{
    [RequiredComponent(typeof(UpdateComponent))]
    public class UserInterfaceComponent : ContentComponent
    {
        public IUserInterfaceState UserInterfaceState { get; private set; }

        public Overlay Overlay { get; set; }

        public override bool IsInited { get { return Overlay.IsInited; } }

        public UserInterfaceComponent()
            : base(ComponentTypeManager.GetType<UserInterfaceComponent>())
        {
        }

        public override void Initialize()
        {
            if (Overlay == null)
                throw new InvalidOperationException(string.Format("'{0}' cannot be null",
                    ReflectionHelper.GetPropertyName((UserInterfaceComponent c) => c.Overlay)));
            UserInterfaceState = Services.GetService<IUserInterfaceState>();
            if (UserInterfaceState == null)
                throw new InvalidOperationException("UserInterfaceState");
            UserInterfaceState.Initialize();
            UserInterfaceState.SetOverlay(Overlay);
        }

        public override bool Validate()
        {
            // TODO add possibility of loading GUI from disk
            return true;
        }

        #region IResourceProvider

        protected override bool ContainsResource(string resourceName)
        {
            IResourceProvider resourceProvider = Overlay;
            return resourceProvider.ContainsResource(resourceName);
        }

        protected override TResource GetResource<TResource>(string resourceName)
        {
            IResourceProvider resourceProvider = Overlay;
            return resourceProvider.GetResource<TResource>(resourceName);
        }

        protected override IEnumerable<IResource> Resources
        {
            get { return ((IResourceProvider) Overlay).Resources; }
        }

        #endregion

    }
}