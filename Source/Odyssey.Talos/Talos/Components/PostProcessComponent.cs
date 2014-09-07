using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics.PostProcessing;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Messages;
using Odyssey.Utilities.Logging;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Odyssey.Talos.Components
{
    [RequiredComponent(typeof(ModelComponent))]
    public partial class PostProcessComponent : ContentComponent, ITechniqueComponent
    {
        private List<PostProcessAction> actions;
        public string TagFilter { get; set; }

        private Technique[] techniques;

        public List<PostProcessAction> Actions
        {
            get { return actions; }
            set
            {
                if (actions != value)
                {
                    actions = value;
                    RaisePropertyChanged("Actions");
                }
            }
        }

        public IEnumerable<Technique> Techniques { get { return techniques; } }

        public PostProcessComponent() : base(ComponentTypeManager.GetType<PostProcessComponent>())
        {
            actions = new List<PostProcessAction>();
        }

        public override bool Validate()
        {
            bool test = actions.Any();
            if (!test)
                LogEvent.Engine.Error("No post process actions defined in [{0}]", Name);

            foreach (var action in actions.Where(action => !string.Equals(action.AssetName, Param.Odyssey) && !Content.Contains(action.AssetName)))
            {
                LogEvent.Engine.Error("Asset [{0}] not found", action.AssetName);
                test = false;
            }

            return test;
        }

        public override bool IsInited { get { return techniques != null && techniques.All(t => t.IsInited); } }

        public override void Initialize()
        {
            var techniquePool = DeviceService.DirectXDevice.TechniquePool;

            var filteredActions = (from a in actions
                where a.AssetName != Param.Odyssey
                select new {Asset = a.AssetName, Technique = a.Technique}).Distinct().ToArray();

            techniques = new Technique[filteredActions.Length];
            for (int i = 0; i < filteredActions.Length; i++)
            {
                var action = filteredActions[i];

                ShaderCollection shaderCollection = Content.Load<ShaderCollection>(action.Asset);

                var mapping = shaderCollection.Get(action.Technique);
                string techniqueKey = string.Format("{0}.{1}", action.Asset, mapping.Name);
                
                if (!techniquePool.ContainsTechnique(techniqueKey))
                {
                    techniques[i] = new Technique(DeviceService.DirectXDevice, techniqueKey, mapping);
                    techniquePool.RegisterTechnique(techniques[i]);
                    // Notifies other systems that a new technique is ready to be initialized
                    Messenger.Send(new ContentMessage<Technique>(Owner, action.Asset, techniques[i]));
                }
                else 
                    techniques[i] = techniquePool.GetTechnique(techniqueKey);
            }
        }

        internal static Texture2DDescription GetTextureDescription(IDirectXDeviceSettings deviceSettings, float scaleFactor = 1)
        {
            var textureDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = deviceSettings.PreferredBackBufferFormat,
                Width = (int)(deviceSettings.PreferredBackBufferWidth * scaleFactor),
                Height = (int)(deviceSettings.PreferredBackBufferHeight * scaleFactor),
                SampleDescription = new SampleDescription(1, 0),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
            };

            return textureDesc;
        }
    }
}
