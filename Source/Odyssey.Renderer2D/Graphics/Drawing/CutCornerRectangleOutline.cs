using System;
using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class CutCornerRectangleOutline : CutCornerRectangleBase
    {
        private GeometryGroup shape;
        private Matrix3x2 transform;

        public float OutlineThickness { get; set; }

        public override void Render()
        {
            Device.Transform = transform;

            Device.FillGeometry(shape, Fill);
            Device.DrawGeometry(shape, Stroke);
        }

        protected internal override void Measure()
        {
            transform = Matrix3x2.Translation(AbsolutePosition);
        }

        protected override void OnInitializing(ControlEventArgs e)
        {
            base.OnInitializing(e);

            Vector2[] innerPoints =
            {
                new Vector2(CutCornerLength, 0),
                    new Vector2(Width - CutCornerLength, 0),
                    new Vector2(Width, CutCornerLength),
                    new Vector2(Width, Height - CutCornerLength),
                    new Vector2(Width-CutCornerLength, Height),
                    new Vector2(CutCornerLength, Height),
                    new Vector2(0, Height - CutCornerLength),
                    new Vector2(0, CutCornerLength)
            };
            float innerOffset = OutlineThickness / (float)Math.Sqrt(2);
            Vector2[] outerPoints =
            {
                new Vector2(2 * innerOffset, innerOffset),
                    new Vector2(Width - 2 * innerOffset, innerOffset),
                    new Vector2(Width - innerOffset, 2 * innerOffset),
                    new Vector2(Width - innerOffset, Height - 2 * innerOffset),
                    new Vector2(Width - 2 * innerOffset, Height - innerOffset),
                    new Vector2(2 * innerOffset, Height - innerOffset),
                    new Vector2(innerOffset, Height - 2 * innerOffset),
                    new Vector2(innerOffset, 2 * innerOffset)
            };

            PolyLine outerShape = ToDispose(PolyLine.New(string.Format("CC.Outer.{0}", Name), Device, innerPoints, FigureBegin.Filled, FigureEnd.Closed));
            PolyLine innerShape = ToDispose(PolyLine.New(string.Format("CC.Inner.{0}", Name), Device, outerPoints, FigureBegin.Filled, FigureEnd.Closed));

            shape = ToDispose(GeometryGroup.New(string.Format("CC.{0}", Name), Device, FillMode.Alternate, new[] { innerShape, outerShape, }));
        }
    }
}