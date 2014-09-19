#region Using Directives

using Odyssey.Animations;
using Odyssey.Engine;
using SharpDX;
using SharpDX.Direct2D1;

#endregion Using Directives

namespace Odyssey.Graphics
{
    public sealed class SolidColorBrush : Brush
    {
        private new readonly SolidColor ColorResource;
        private new readonly SharpDX.Direct2D1.SolidColorBrush Resource;

        private SolidColorBrush(string name, Direct2DDevice device, SolidColor solidColor, SharpDX.Direct2D1.SolidColorBrush brush)
            : base(name, device, solidColor, brush)
        {
            ColorResource = solidColor;
            Resource = brush;
        }

        [Animatable]
        public Color4 Color
        {
            get { return ColorResource.Color; }
            set
            {
                if (ColorResource.Color == value)
                    return;
                ColorResource.Color = value;
                Resource.Color = value;
            }
        }

        public static SolidColorBrush New(string name, Direct2DDevice device, SolidColor solidColorBrush)
        {
            var brushProperties = new BrushProperties() { Opacity = solidColorBrush.Opacity };
            return new SolidColorBrush(name, device, solidColorBrush, new SharpDX.Direct2D1.SolidColorBrush(device, solidColorBrush.Color, brushProperties));
        }
    }
}