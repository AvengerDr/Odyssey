﻿using Odyssey.Content;
using Odyssey.Core;
using Odyssey.Engine;
using SharpDX.DirectWrite;

namespace Odyssey.UserInterface.Style
{
    public class TextFormat : Component, IInitializable, IResource
    {
        protected SharpDX.DirectWrite.TextFormat Resource ;

        public bool IsInited { get; private set; }

        protected TextFormat(string name, SharpDX.DirectWrite.TextFormat textFormat) : base(name)
        {
            Resource = textFormat;
        }

        /// <summary>
        /// Initializes the specified device local.
        /// </summary>
        /// <param name="resource">The resource.</param>
        protected virtual void Initialize(SharpDX.DirectWrite.TextFormat resource)
        {
            Resource = ToDispose(resource);
            if (resource != null)
            {
                resource.Tag = this;
            }
            IsInited = true;
        }

        public void Initialize()
        {
            Initialize(ToDispose(Resource));
        }

        public static implicit operator SharpDX.DirectWrite.TextFormat(TextFormat from)
        {
            return from == null ? null : from.Resource;
        }

        public static TextFormat New(IServiceRegistry services, TextStyle textStyle)
        {
            var deviceService = services.GetService<IDirect2DService>();
            var content = services.GetService<IAssetProvider>();

            SharpDX.DirectWrite.TextFormat textFormat;
            float scaledFontSize = textStyle.Size*LayoutManager.Scale;

            if (content.Contains(textStyle.FontFamily))
                textFormat = new SharpDX.DirectWrite.TextFormat(deviceService.Direct2DDevice, textStyle.FontFamily, services.GetService<IStyleService>().FontCollection,
                    (SharpDX.DirectWrite.FontWeight)textStyle.FontWeight, (SharpDX.DirectWrite.FontStyle)textStyle.FontStyle, FontStretch.Normal, scaledFontSize);
            else textFormat = new SharpDX.DirectWrite.TextFormat(deviceService.Direct2DDevice, textStyle.FontFamily, (SharpDX.DirectWrite.FontWeight)textStyle.FontWeight,
                (SharpDX.DirectWrite.FontStyle)textStyle.FontStyle, scaledFontSize);

            textFormat.TextAlignment = (SharpDX.DirectWrite.TextAlignment)textStyle.TextAlignment;
            textFormat.ParagraphAlignment = (SharpDX.DirectWrite.ParagraphAlignment)textStyle.ParagraphAlignment;
            
            return new TextFormat(textStyle.Name, textFormat);
        }

    }
}
