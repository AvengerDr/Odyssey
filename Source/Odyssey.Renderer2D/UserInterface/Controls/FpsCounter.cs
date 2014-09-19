using System;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.UserInterface.Style;
using SharpDX;
using SharpDX.Direct2D1;
using SolidColorBrush = Odyssey.Graphics.SolidColorBrush;

namespace Odyssey.UserInterface.Controls
{
    public class FpsCounter : Label
    {
        private readonly FpsTimer timer;

        public FpsCounter()
        {
            timer = new FpsTimer();
        }

        public override void Render()
        {
            if (!IsVisible)
                return;

            timer.Measure();
            Device.Transform = Matrix.Identity;
            Device.DrawText(string.Format("{0:F2} FPS ({1:F1} ms)", timer.MeasuredFPS, timer.FrameTime * 1000.0), TextFormat, new RectangleF(8, 8, 8 + 256, 8 + 16), Foreground);
        }

        protected override void OnInitializing(EventArgs e)
        {
            Foreground = ToDispose(SolidColorBrush.New(string.Format("FPSCounter.{0}", Name), Device, new SolidColor("White", Color.White)));
            Foreground.Initialize();
            TextFormat = ToDispose(TextFormat.New(Device.Services, Overlay.Theme.GetResource<TextStyle>("Small")));
            Device.SetTextAntialias(AntialiasMode.Aliased);
        }
    }
}