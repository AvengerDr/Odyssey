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

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Odyssey.Animations;
using Odyssey.UserInterface;
using Odyssey.UserInterface.Controls;
using Odyssey.UserInterface.Serialization;
using Odyssey.UserInterface.Style;
using Odyssey.Utilities.Reflection;
using Odyssey.Utilities.Text;
using SharpDX;

#endregion Using Directives

namespace Odyssey.Graphics.Drawing
{
    public abstract class Shape : UIElement, IShape, IRequiresCaching
    {
        private string fillBrushClass;
        private string strokeBrushClass;
        private Brush fill;
        private Brush stroke;

        [Animatable]
        [CacheAnimation(typeof(GradientStop), "Color")]
        public Brush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
            }
        }

        [Animatable]
        [CacheAnimation(typeof(GradientStop), "Color")]
        public Brush Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }

        public float StrokeThickness { get; set; }

        protected internal override UIElement Copy()
        {
            Shape copy = (Shape)base.Copy();
            copy.fillBrushClass = fillBrushClass;
            copy.strokeBrushClass = strokeBrushClass;
            copy.StrokeThickness = StrokeThickness;
            return copy;
        }

        private Brush CreateOrRetrieveBrush(string brushClass)
        {
            var styleService = Device.Services.GetService<IStyleService>();
            if (string.IsNullOrEmpty(brushClass))
            {
                return null;
            }

            var brushResource = Overlay.Theme.GetResource<ColorResource>(brushClass);

            return styleService.CreateOrRetrieveColorResource(brushResource);
        }

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);

            if (fill == null)
                fill = CreateOrRetrieveBrush(fillBrushClass);

            if (stroke == null)
                stroke = CreateOrRetrieveBrush(strokeBrushClass);
        }

        protected override void OnLayoutUpdated(EventArgs e)
        {
            base.OnLayoutUpdated(e);
            if (fill != null)
                fill.Transform = Matrix3x2.Scaling(Width, Height) * Transform;
        }

        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            var reader = e.XmlReader;
            string strokeThickness = reader.GetAttribute("StrokeThickness");
            StrokeThickness = string.IsNullOrEmpty(strokeThickness) ? 0 : float.Parse(strokeThickness, CultureInfo.InvariantCulture);

            string sFill = reader.GetAttribute("Fill");
            string sStroke = reader.GetAttribute("Stroke");
            if (!string.IsNullOrEmpty(sFill))
            {
                fillBrushClass = Text.ParseResource(sFill);
            }
            if (!string.IsNullOrEmpty(sStroke))
            {
                strokeBrushClass = Text.ParseResource(sStroke);
            }

            if (!reader.IsEmptyElement)
                reader.ReadEndElement();
        }

        #region IRequiresCaching
        public IAnimationCurve CacheAnimation(Type type, string propertyName, IAnimationCurve animationCurve)
        {
            // Checks whether the curve affects a property marked with the CacheAnimationAttribute
            var property = (from p in ReflectionHelper.GetProperties(GetType())
                            let attributes = p.GetCustomAttributes<CacheAnimationAttribute>()
                            from attribute in attributes
                            where attribute != null && attribute.Type == type && attribute.PropertyName == propertyName
                            select p).FirstOrDefault();

            if (property != null)
            {
                object value = property.GetValue(this);

                var bGradient = value as GradientBrush;
                if (value == null)
                    throw new NotImplementedException("Solid Color animation not yet supported");
                return GradientBrushCurve.FromColor4Curve(Device, (Color4Curve)animationCurve, bGradient);
            }

            return null;
        }
        #endregion
    }
}