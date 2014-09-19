#region Using Directives

using Odyssey.Engine;
using Odyssey.UserInterface.Controls;
using SharpDX;

#endregion Using Directives

namespace MiniUI
{
    public static class SampleOverlay
    {
        public static Overlay New(IServiceRegistry services)
        {
            IDirectXDeviceSettings settings = services.GetService<IDirectXDeviceSettings>();
            var overlay = new Overlay(services)
            {
                Width = settings.PreferredBackBufferWidth,
                Height = settings.PreferredBackBufferHeight
            };

            // signal that we are starting to design the UI
            overlay.BeginDesign();

            Panel panel1 = new Panel { Position = new Vector2(128, 128), Width = 480, Height = 320 };
            Panel panel2 = new Panel { Position = new Vector2(480, 64), Width = 256, Height = 200 };
            Label label = new Label { Position = new Vector2(8, 8), Text = "I'm a label.", Width = 200, Height = 48 };
            Button button = new Button { Position = new Vector2(16, 64), Content = new Label { Text = "Button", TextStyleClass = "SmallCenter" } };
            FpsCounter fpsCounter = new FpsCounter { Position = new Vector2(16, 16) };
            button.Tap += (s, eventArgs) => ((Label)button.Content).Text = "It works!";

            panel1.Add(label);
            overlay.Add(panel2);
            overlay.Add(panel1);
            overlay.Add(fpsCounter);
            overlay.Add(button);

            // we're done: BeginDesign() and EndDesign() are required for correct initialization
            overlay.EndDesign();

            return overlay;
        }
    }
}