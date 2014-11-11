﻿using Odyssey.Engine;
using SharpDX.Direct3D11;

namespace Odyssey.Graphics
{
    /// <summary>
    /// Blend state collection
    /// </summary>
    public sealed class BlendStateCollection : StateCollectionBase<BlendState>
    {
        /// <summary>
        /// A built-in state object with settings for additive blend, that is adding the destination data to the source data without using alpha.
        /// </summary>
        public readonly BlendState Additive;

        /// <summary>
        /// A built-in state object with settings for alpha blend, that is blending the source and destination data using alpha.
        /// </summary>
        public readonly BlendState AlphaBlend;

        /// <summary>
        /// A built-in state object with settings for blending with non-premultiplied alpha, that is blending source and destination data using alpha while assuming the color data contains no alpha information.
        /// </summary>
        public readonly BlendState NonPremultiplied;

        /// <summary>
        /// A built-in state object with settings for opaque blend, that is overwriting the source with the destination data.
        /// </summary>
        public readonly BlendState Opaque;

        /// <summary>
        /// A built-in default state object (no blending).
        /// </summary>
        public readonly BlendState Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlendStateCollection" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        internal BlendStateCollection(DirectXDevice device)
            : base(device)
        {
            Additive = Add(BlendState.New(device, "Additive", BlendOption.SourceAlpha, BlendOption.One));
            AlphaBlend = Add(BlendState.New(device, "AlphaBlend", BlendOption.One, BlendOption.InverseSourceAlpha));
            NonPremultiplied = Add(BlendState.New(device, "NonPremultiplied", BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha));
            Opaque = Add(BlendState.New(device, "Opaque", BlendOption.One, BlendOption.Zero));
            Default = Add(BlendState.New(device, "Default", BlendStateDescription.Default()));

            foreach (var state in Items)
                state.Initialize();
        }

    }
}
