#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
//
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
//
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion License

#region Using Directives

using GalaSoft.MvvmLight.Messaging;
using Odyssey.Daedalus.ViewModel.Messages;
using Odyssey.Engine;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;
using Odyssey.Talos.Components;
using Odyssey.Talos.Systems;
using SharpDX;
using SharpDX.Diagnostics;
using SharpDX.Direct3D11;
using System;

#endregion Using Directives

namespace Odyssey.Daedalus.Viewer
{
    public class DirectXViewer : DesktopWpfApplication
    {
        private readonly Lazy<VSGraphicsDebugger> vsDebugger;
        private ShaderComponent cShader;
        private bool captureNextFrame;
        private VSGraphicsDebugger.CaptureToken captureToken;
        private Entity previewModel;
        private Scene scene;
        private ShaderCollection shaderCollection;
        private VertexShaderFlags vsFlags;

        public DirectXViewer()
        {
            DirectXDeviceManager directXDeviceManager = new DirectXDeviceManager(this);
            directXDeviceManager.DeviceCreationFlags |= DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug;
            directXDeviceManager.PreferredBackBufferWidth = 576;
            directXDeviceManager.PreferredBackBufferHeight = 576;
            directXDeviceManager.PreferMultiSampling = true;
            directXDeviceManager.PreferredMultiSampleCount = 1;

            vsDebugger = new Lazy<VSGraphicsDebugger>(() => new VSGraphicsDebugger("Debug.vsglog"));
            Messenger.Default.Register<ViewerMessage>(this, ReceiveMessage);
        }

        public bool IsInited { get; private set; }

        public void SetTechnique(ShaderCollection shaderCollection, string techniqueKey)
        {
            this.shaderCollection = shaderCollection;
            Content.Store(shaderCollection.Name, shaderCollection);
            cShader = new ShaderComponent {AssetName = shaderCollection.Name, Key = techniqueKey};
            vsFlags = shaderCollection[techniqueKey].Key.VertexShader;
        }

        protected override bool BeginDraw()
        {
            if (captureNextFrame)
            {
                captureToken = vsDebugger.Value.BeginCapture();
            }
            return base.BeginDraw();
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            if (captureNextFrame)
            {
                captureToken.Dispose();
                captureNextFrame = false;
            }
        }

        protected override void Initialize()
        {
            Content.LoadAssetList("Assets/Assets.yaml");
            DefineScene();
            SetUpdateCallback(scene);
            Window.IsMouseVisible = true;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            scene.Unload();
        }

        private void DefineScene()
        {
            var device = Services.GetService<IOdysseyDeviceService>().DirectXDevice;
            scene = new Scene(Services);

            var cube = CubeMesh.New(device, modelOperations: ModelAnalyzer.DetectModelRequirements(vsFlags));
            Content.Store(cube.Name, cube);

            var modelSystem = new ContentLoadingSystem();
            var sInitialization = new InitializationSystem();
            var cameraSystem = new PerspectiveCameraSystem();
            var lightSystem = new PointLightSystem();
            var positionSystem = new TransformSystem();
            var sOptimization = new OptimizationSystem();
            var sRendering = new RenderSystem();
            var sUI = new UserInterfaceSystem();

            scene.BeginDesign();

            scene.AddSystem(modelSystem);
            scene.AddSystem(cameraSystem);
            scene.AddSystem(lightSystem);
            scene.AddSystem(sInitialization);
            scene.AddSystem(positionSystem);
            scene.AddSystem(sOptimization);
            scene.AddSystem(sRendering);
            scene.AddSystem(sUI);

            Entity root = scene.CreateEntity("Root");
            root.RegisterComponent(new TransformComponent());
            root.RegisterComponent(new PositionComponent());

            Entity camera = scene.CreateEntity("Camera");
            camera.RegisterComponent(new CameraComponent {});
            camera.RegisterComponent(new PositionComponent {Position = new Vector3(0, 0f, 3f)});
            camera.RegisterComponent(new UpdateComponent {UpdateFrequency = UpdateFrequency.RealTime});

            Entity pointLight = scene.CreateEntity("PointLight");
            pointLight.RegisterComponent(new PointLightComponent {});
            pointLight.RegisterComponent(new PositionComponent {Position = new Vector3(0, 0f, 3f)});
            pointLight.RegisterComponent(new UpdateComponent {UpdateFrequency = UpdateFrequency.RealTime});
            pointLight.RegisterComponent(new ParentComponent() {Parent = root});
            pointLight.RegisterComponent(new TransformComponent());

            previewModel = scene.CreateEntity("PreviewModel");
            previewModel.RegisterComponent(new PositionComponent());
            previewModel.RegisterComponent(new TransformComponent());
            previewModel.RegisterComponent(new ModelComponent() {AssetName = cube.Name});
            previewModel.RegisterComponent(new UpdateComponent() {UpdateFrequency = UpdateFrequency.RealTime});
            previewModel.RegisterComponent(new MaterialComponent() {Diffuse = Color.Blue});
            previewModel.RegisterComponent(cShader);

            Entity ui = scene.CreateEntity("UI");
            ui.RegisterComponent(new UserInterfaceComponent {Overlay = Viewer.CreateOverlay(Services, this)});
            ui.RegisterComponent(new UpdateComponent() {UpdateFrequency = UpdateFrequency.RealTime});

            scene.EndDesign();
        }

        private void ReceiveMessage(ViewerMessage viewerMessage)
        {
            switch (viewerMessage.Action)
            {
                case ViewerAction.None:
                    break;

                case ViewerAction.CaptureFrame:
                    captureNextFrame = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("viewerMessage");
            }
        }
    }
}