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

using Odyssey.Animations;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;
using Odyssey.Utilities.Reflection;
using SharpDX;
using System;
using SharpDX.Direct3D11;
using SharpDX.IO;
using Texture2D = Odyssey.Graphics.Texture2D;

#endregion Using Directives

namespace Odyssey
{
    public abstract class Application : Component, IWindowService
    {
        private readonly ApplicationTime appTime;
        private readonly ApplicationPlatform applicationPlatform;
        private readonly IAssetProvider contentManager;
        private readonly int[] lastUpdateCount;
        private readonly TimeSpan maximumElapsedTime;
        private readonly IServiceRegistry services;
        private readonly TimerTick timer;
        private readonly float updateCountAverageSlowLimit;
        private TimeSpan accumulatedElapsedGameTime;
        private IDirectXDeviceManager deviceManager;
        private IDirectXDeviceService deviceService;
        private bool drawRunningSlowly;
        private bool forceElapsedTimeToZero;
        private bool isEndRunRequired;
        private bool isExiting;
        private bool isFirstUpdateDone;
        private TimeSpan lastFrameElapsedAppTime;
        private int nextLastUpdateCountIndex;
        private IUpdateable updateCallback;
        private IRenderable renderCallback;
        private bool suppressDraw;
        private TimeSpan totalTime;

        internal Application()
        {
            var platformTypeAttribute = ReflectionHelper.GetAttribute<PlatformTypeAttribute>(GetType());
            if (!ReflectionHelper.IsTypeDerived(platformTypeAttribute.PlatformType, typeof (ApplicationPlatform)))
                throw new InvalidOperationException("PlatformType must be derived from ApplicationPlatform.");

            services = new ServiceRegistry();
            appTime = new ApplicationTime();
            totalTime = new TimeSpan();
            timer = new TimerTick();
            IsFixedTimeStep = false;
            maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
            TargetElapsedTime = TimeSpan.FromTicks(10000000/120); // target elapsed time is by default 60Hz
            lastUpdateCount = new int[4];
            nextLastUpdateCountIndex = 0;

            // Calculate the updateCountAverageSlowLimit (assuming moving average is >=3 )
            // Example for a moving average of 4:
            // updateCountAverageSlowLimit = (2 * 2 + (4 - 2)) / 4 = 1.5f
            const int BadUpdateCountTime = 2; // number of bad frame (a bad frame is a frame that has at least 2 updates)
            var maxLastCount = 2*Math.Min(BadUpdateCountTime, lastUpdateCount.Length);
            updateCountAverageSlowLimit = (float) (maxLastCount + (lastUpdateCount.Length - maxLastCount))/lastUpdateCount.Length;

            services.AddService(typeof (IWindowService), this);
            services.AddService(typeof (ITimeService), appTime);

            // Setup Content Manager
            contentManager = new ContentManager(services);
            contentManager.AddMapping(AssetType.EngineReferences, typeof(EngineReferenceCollection));
            contentManager.AddMapping(AssetType.Model, typeof(Model));
            contentManager.AddMapping(AssetType.Effect, typeof(ShaderCollection));
            contentManager.AddMapping(AssetType.Texture2D, typeof(Texture2D));
            contentManager.AddMapping(AssetType.TextureCube, typeof(TextureCube));
            contentManager.AddMapping(AssetType.Cutscene, typeof(Cutscene));

            var additionalServices = ReflectionHelper.GetAttributes<RequiredServiceAttribute>(GetType());
            foreach (var requiredService in additionalServices)
            {
                var service = Activator.CreateInstance(requiredService.ClassType, services);
                services.AddService(requiredService.ServiceType, service);
            }

            // Setup Platform
            applicationPlatform = (ApplicationPlatform) Activator.CreateInstance(platformTypeAttribute.PlatformType, this);
            applicationPlatform.Activated += ApplicationPlatformActivated;
            applicationPlatform.Deactivated += ApplicationPlatformDeactivated;
            applicationPlatform.Exiting += ApplicationPlatform_Exiting;
            applicationPlatform.WindowCreated += ApplicationPlatformWindowCreated;
        }

        public IAssetProvider Content
        {
            get { return contentManager; }
        }

        public ApplicationContext Context { get; private set; }

        /// <summary>
        /// Gets or sets the inactive sleep time.
        /// </summary>
        /// <value>The inactive sleep time.</value>
        public TimeSpan InactiveSleepTime { get; set; }

        public bool IsActive { get; private set; }

        public bool IsFixedTimeStep { get; set; }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        public IServiceRegistry Services
        {
            get { return services; }
        }

        public TimeSpan TargetElapsedTime { get; set; }

        /// <summary>
        /// Gets the abstract window.
        /// </summary>
        /// <value>The window.</value>
        public ApplicationWindow Window
        {
            get { return applicationPlatform != null ? applicationPlatform.MainWindow : null; }
        }

        object IWindowService.NativeWindow
        {
            get { return Window.NativeWindow; }
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void Exit()
        {
            isExiting = true;
            applicationPlatform.Exit();
            if (IsRunning && isEndRunRequired)
            {
                EndRun();
                IsRunning = false;
            }
        }

        /// <summary>
        /// Resets the elapsed time counter.
        /// </summary>
        public void ResetElapsedTime()
        {
            forceElapsedTimeToZero = true;
            drawRunningSlowly = false;
            Array.Clear(lastUpdateCount, 0, lastUpdateCount.Length);
            nextLastUpdateCountIndex = 0;
        }

        /// <summary>
        /// Call this method to initialize the game, begin running the game loop, and start processing events for the game.
        /// </summary>
        /// <param name="applicationContext">The window Context for this game.</param>
        /// <exception cref="System.InvalidOperationException">Cannot run this instance while it is already running</exception>
        public void Run(ApplicationContext applicationContext = null)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot run this instance while it is already running");
            }

            // Gets the graphics device manager
            deviceManager = Services.GetService(typeof (IDirectXDeviceManager)) as IDirectXDeviceManager;
            if (deviceManager == null)
            {
                throw new InvalidOperationException("No DeviceManager found");
            }

            // Gets the GameWindow Context
            Context = applicationContext;

            try
            {
                applicationPlatform.Run(applicationContext);

                if (applicationPlatform.IsBlockingRun)
                {
                    // If the previous call was blocking, then we can call Endrun
                    EndRun();
                }
                else
                {
                    // EndRun will be executed on Game.Exit
                    isEndRunRequired = true;
                }
            }
            finally
            {
                if (!isEndRunRequired)
                {
                    IsRunning = false;
                }
            }
        }

        public void SetUpdateCallback(IUpdateable updateCallback)
        {
            this.updateCallback = updateCallback;
        }

        public void SetRenderCallback(IRenderable renderCallback)
        {
            this.renderCallback = renderCallback;
        }

        /// <summary>
        /// Prevents calls to Draw until the next Update.
        /// </summary>
        public void SuppressDraw()
        {
            suppressDraw = true;
        }

        /// <summary>
        /// Updates the game's clock and calls Update and Draw.
        /// </summary>
        public void Tick()
        {
            // If this instance is existing, then don't make any further update/draw
            if (isExiting)
            {
                return;
            }

            // If this instance is not active, sleep for an inactive sleep time
            if (!IsActive)
            {
                SharpDX.Utilities.Sleep(InactiveSleepTime);
            }

            // Update the timer
            timer.Tick();

            var elapsedAdjustedTime = timer.ElapsedAdjustedTime;

            if (forceElapsedTimeToZero)
            {
                elapsedAdjustedTime = TimeSpan.Zero;
                forceElapsedTimeToZero = false;
            }

            if (elapsedAdjustedTime > maximumElapsedTime)
            {
                elapsedAdjustedTime = maximumElapsedTime;
            }

            bool suppressNextDraw = true;
            int updateCount = 1;
            var singleFrameElapsedTime = elapsedAdjustedTime;

            if (IsFixedTimeStep)
            {
                // If the rounded TargetElapsedTime is equivalent to current ElapsedAdjustedTime
                // then make ElapsedAdjustedTime = TargetElapsedTime. We take the same internal rules as XNA
                if (Math.Abs(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks) < (TargetElapsedTime.Ticks >> 6))
                {
                    elapsedAdjustedTime = TargetElapsedTime;
                }

                // Update the accumulated time
                accumulatedElapsedGameTime += elapsedAdjustedTime;

                // Calculate the number of update to issue
                updateCount = (int) (accumulatedElapsedGameTime.Ticks/TargetElapsedTime.Ticks);

                // If there is no need for update, then exit
                if (updateCount == 0)
                {
                    // check if we can sleep the thread to free CPU resources
                    var sleepTime = TargetElapsedTime - accumulatedElapsedGameTime;
                    if (sleepTime > TimeSpan.Zero)
                        SharpDX.Utilities.Sleep(sleepTime);
                    return;
                }

                // Calculate a moving average on updateCount
                lastUpdateCount[nextLastUpdateCountIndex] = updateCount;
                float updateCountMean = 0;
                for (int i = 0; i < lastUpdateCount.Length; i++)
                {
                    updateCountMean += lastUpdateCount[i];
                }

                updateCountMean /= lastUpdateCount.Length;
                nextLastUpdateCountIndex = (nextLastUpdateCountIndex + 1)%lastUpdateCount.Length;

                // Test when we are running slowly
                drawRunningSlowly = updateCountMean > updateCountAverageSlowLimit;

                // We are going to call Update updateCount times, so we can subtract this from accumulated elapsed game time
                accumulatedElapsedGameTime = new TimeSpan(accumulatedElapsedGameTime.Ticks - (updateCount*TargetElapsedTime.Ticks));
                singleFrameElapsedTime = TargetElapsedTime;
            }
            else
            {
                Array.Clear(lastUpdateCount, 0, lastUpdateCount.Length);
                nextLastUpdateCountIndex = 0;
                drawRunningSlowly = false;
            }

            // Reset the time of the next frame
            for (lastFrameElapsedAppTime = TimeSpan.Zero; updateCount > 0 && !isExiting; updateCount--)
            {
                appTime.Update(totalTime, singleFrameElapsedTime, drawRunningSlowly);
                try
                {
                    updateCallback.Update(appTime);

                    // If there is no exception, then we can draw the frame
                    suppressNextDraw &= suppressDraw;
                    suppressDraw = false;
                }
                finally
                {
                    lastFrameElapsedAppTime += singleFrameElapsedTime;
                    totalTime += singleFrameElapsedTime;
                }
            }

            if (!suppressNextDraw)
            {
                RenderFrame();
            }
        }

        internal void InitializeBeforeRun()
        {
            // Make sure that the device is already created
            deviceManager.CreateDevice();

            // Gets the graphics device service
            deviceService = Services.GetService(typeof (IDirectXDeviceService)) as IDirectXDeviceService;
            if (deviceService == null)
                throw new InvalidOperationException("No deviceService found");

            // Checks the graphics device
            if (deviceService.DirectXDevice == null)
                throw new InvalidOperationException("No device found");

            // Initialize this instance and all game systems
            Initialize();

            IsRunning = true;

            //BeginRun();

            timer.Reset();
            appTime.Update(totalTime, TimeSpan.Zero, false);
            appTime.FrameCount = 0;

            // Run an update for the first time 
            updateCallback.Update(appTime);
            isFirstUpdateDone = true;
        }

        /// <summary>
        /// Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns><c>true</c> to continue drawing, false to not call <see cref="Draw"/> and <see cref="EndDraw"/></returns>
        protected virtual bool BeginDraw()
        {
            if ((deviceManager != null) && !deviceManager.BeginDraw())
            {
                return false;
            }

            return true;
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (this)
                {

                    var disposableDeviceManager = deviceManager as IDisposable;
                    if (disposableDeviceManager != null)
                    {
                        disposableDeviceManager.Dispose();
                    }

                    DisposeDeviceEvents();

                    if (applicationPlatform != null)
                    {
                        applicationPlatform.Dispose();
                    }

                }
            }

            base.Dispose(disposeManagedResources);
        }

        protected virtual void EndDraw()
        {
            if (deviceManager != null)
            {
                deviceManager.EndDraw();
            }
        }

        /// <summary>Called after the game loop has stopped running before exiting.</summary>
        protected virtual void EndRun()
        {
        }

        protected virtual void Initialize()
        {
            const string filePath = Global.DataPath + "System.yaml";
            const string references = "EngineReferences";
            if (!NativeFile.Exists(filePath))
                throw new InvalidOperationException(string.Format("Odyssey System Data not found: check if {0} exists",filePath));

            contentManager.LoadAssetList(filePath);

            var refData = contentManager.Load<EngineReferenceCollection>(references);
            services.AddService(typeof(IReferenceService), refData);
            SetupDeviceEvents();
        }

        private void deviceService_DeviceCreated(object sender, EventArgs e)
        {
            
        }

        private void deviceService_DeviceDisposing(object sender, EventArgs e)
        {
            Content.Unload();
            applicationPlatform.MainWindow.Dispose();
        }

        private void DisposeDeviceEvents()
        {
            if (deviceService != null)
            {
                deviceService.DeviceCreated -= deviceService_DeviceCreated;
                deviceService.DeviceDisposing -= deviceService_DeviceDisposing;
            }
        }

        private void RenderFrame()
        {
            try
            {
                if (!isExiting && isFirstUpdateDone && !Window.IsMinimized && BeginDraw())
                {
                    appTime.Update(totalTime, lastFrameElapsedAppTime, drawRunningSlowly);
                    appTime.FrameCount++;

                    renderCallback.Render(appTime);

                    EndDraw();
                }
            }
            finally
            {
                lastFrameElapsedAppTime = TimeSpan.Zero;
            }
        }

        private void SetupDeviceEvents()
        {
            // Find the IGraphicsDeviceSerive.
            deviceService = Services.GetService(typeof(IDirectXDeviceService)) as IDirectXDeviceService;

            // If there is no graphics device service, don't go further as the whole Game would not work
            if (deviceService == null)
            {
                throw new InvalidOperationException("Unable to create find a IGraphicsDeviceService");
            }

            if (deviceService.DirectXDevice == null)
            {
                throw new InvalidOperationException("Unable to find a Device instance");
            }

            deviceService.DeviceCreated += deviceService_DeviceCreated;
            deviceService.DeviceDisposing += deviceService_DeviceDisposing;
        }

        #region Events

        /// <summary>
        /// Occurs when [activated].
        /// </summary>
        public event EventHandler<EventArgs> Activated;

        /// <summary>
        /// Occurs when [deactivated].
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;

        /// <summary>
        /// Occurs when [exiting].
        /// </summary>
        public event EventHandler<EventArgs> Exiting;

        /// <summary>
        /// Occurs when [window created].
        /// </summary>
        public event EventHandler<EventArgs> WindowCreated;

        /// <summary>
        /// Raises the Activated event. Override this method to add code to handle when the game gains focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Activated event.</param>
        protected virtual void OnActivated(object sender, EventArgs args)
        {
            var handler = Activated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the Deactivated event. Override this method to add code to handle when the game loses focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Deactivated event.</param>
        protected virtual void OnDeactivated(object sender, EventArgs args)
        {
            var handler = Deactivated;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises an Exiting event. Override this method to add code to handle when the game is exiting.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Exiting event.</param>
        protected virtual void OnExiting(object sender, EventArgs args)
        {
            var handler = Exiting;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnWindowCreated()
        {
            EventHandler<EventArgs> handler = WindowCreated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Events

        #region ApplicationPlatform events

        private void ApplicationPlatform_Exiting(object sender, EventArgs e)
        {
            Content.Unload();
            OnExiting(this, EventArgs.Empty);
        }

        private void ApplicationPlatformActivated(object sender, EventArgs e)
        {
            if (!IsActive)
            {
                IsActive = true;
                OnActivated(this, EventArgs.Empty);
            }
        }

        private void ApplicationPlatformDeactivated(object sender, EventArgs e)
        {
            if (IsActive)
            {
                IsActive = false;
                OnDeactivated(this, EventArgs.Empty);
            }
        }

        private void ApplicationPlatformWindowCreated(object sender, EventArgs eventArgs)
        {
            OnWindowCreated();
        }

        #endregion ApplicationPlatform events
    }
}