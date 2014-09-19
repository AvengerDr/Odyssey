using System;
using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class Path : Shape
    {
        public PathGeometry Data { get; set; }

        public override bool Contains(Vector2 cursorLocation)
        {
            return false;
        }

        public override void Render()
        {
            //Device.DrawGeometry(Data, Stroke);
            Device.FillGeometry(Data, Fill);
        }

        protected override void OnInitializing(EventArgs e)
        {
            if (Data == null)
                throw new InvalidOperationException("'Data' cannot be null");
            ToDispose(Data).Initialize();
        }
    }
}