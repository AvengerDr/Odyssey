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

using System;
using Odyssey.Graphics;
using Odyssey.UserInterface.Style;
using Odyssey.Utilities.Reflection;
using SharpDX.Direct2D1;
using Brush = Odyssey.Graphics.Brush;
using TextFormat = Odyssey.UserInterface.Style.TextFormat;

namespace Odyssey.UserInterface.Controls
{
    public abstract class LabelBase : Control
    {
        protected const string DefaultTextClass = "Default";
        private const string ControlTag = "Default";
        private string text;
        private TextFormat textFormat;

        protected TextFormat TextFormat
        {
            get { return textFormat; }
            set { textFormat = value; }
        }

        public Brush Foreground { get; set; }

        public event EventHandler<TextEventArgs> TextChanged;

        protected LabelBase()
            : this(ControlTag)
        {
        }

        protected LabelBase(string textDefinitionClass)
            : base("Empty", textDefinitionClass)
        {
            CanRaiseEvents = false;
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (!string.Equals(text, value))
                {
                    string oldValue = text;
                    text = value;
                    OnTextChanged(new TextEventArgs(text, oldValue));
                }
            }
        }

        protected virtual void OnTextChanged(TextEventArgs e)
        {
            RaiseEvent(TextChanged, this, e);
        }

        protected override void OnInitializing(EventArgs e)
        {
            if (TextStyle == null)
                ApplyTextDescription();
            
            if (string.IsNullOrEmpty(Text))
                Text = Name;
        }

        protected override void OnTextStyleChanged(EventArgs e)
        {
            base.OnTextStyleChanged(e);
            var styleService = Overlay.Services.GetService<IStyleService>();
            if (Foreground == null)
            {
                var brushResource = Overlay.Theme.GetResource<ColorResource>(TextStyle.Foreground);
                Foreground = styleService.CreateOrRetrieveColorResource(brushResource);
            }

            textFormat = styleService.CreateOrRetrieveTextResource(TextStyle);
            DeviceContext context = Device;
            context.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Grayscale;
        }
    }
}