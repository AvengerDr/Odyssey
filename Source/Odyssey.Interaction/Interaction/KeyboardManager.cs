using System;
using System.Diagnostics.Contracts;
using SharpDX;

namespace Odyssey.Interaction
{
    /// <summary>
    /// Provides access to keyboard state
    /// </summary>
    public class KeyboardManager : Component,IKeyboardService
    {
        private readonly IServiceRegistry services;
        private KeyboardState state;
        private KeyboardState nextState;

        // provides platform-specific bindings to keyboard events
        private KeyboardPlatform platform;

        /// <summary>
        /// Creates a new instance of <see cref="KeyboardManager"/> class.
        /// </summary>
        /// <param name="services">A reference to the <see cref="IServiceRegistry"/> used by this application.</param>
        /// <exception cref="ArgumentNullException">Is thrown if <paramref name="services"/> is null</exception>
        public KeyboardManager(IServiceRegistry services)
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");
            this.services = services;
            this.services.AddService(typeof(IKeyboardService), this);
        }

        internal void Associate(KeyboardPlatform platform)
        {
            this.platform = platform;
            platform.KeyDown += HandleKeyDown;
            platform.KeyUp += HandleKeyUp;
        }

        /// <summary>
        /// Gets current keyboard state.
        /// </summary>
        /// <returns>A snapshot of current keyboard state</returns>
        public KeyboardState GetState()
        {
            return state;
        }

        /// <summary>
        /// Handles the <see cref="KeyboardPlatform.KeyDown"/> event
        /// </summary>
        /// <param name="key">The pressed key</param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyData;
            var keyState = nextState[key];

            if (!keyState.Down)
            {
                keyState.Pressed = true;
            }
            keyState.Down = true;
            nextState[key] = keyState;
        }

        /// <summary>
        /// Handles the <see cref="KeyboardPlatform.KeyUp"/> event
        /// </summary>
        /// <param name="key">The released key</param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            var key = e.KeyData;
            nextState[key] = ButtonStateFlags.Released;
        }

        public void Update()
        {
            state = nextState;

            nextState.ResetPressedReleased();
        }
    }
}
