using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Geometry;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public partial class Designer
    {
        private delegate void RenderRectangle(RectangleF rectangle);
        void CreatePolyLine(IEnumerable<Vector2> points, float lineWidth, RenderRectangle callback)
        {
            Contract.Requires<ArgumentNullException>(points!=null, "points");
            Vector2[] pointArray = points as Vector2[] ?? points.ToArray();
            if (pointArray.Length < 2)
                throw new InvalidOperationException("At least two points are required");

            for (int i = 0; i < pointArray.Length - 1; i++)
            {
                Vector2 p0xy = pointArray[i];
                Vector2 p1xy = pointArray[i + 1];
                Vector3 p0 = new Vector3(p0xy, 0);
                Vector3 p1 = new Vector3(p1xy, 0);
                float d = Vector3.Distance(p0, p1);

                float xDiff = p1.X - p0.X;
                float yDiff = p1.Y - p0.Y;
                float angle;

                if (MathHelper.IsCloseToZero(xDiff))
                    angle = 0;
                else
                    angle = (float) Math.Atan2(yDiff, xDiff) - MathHelper.PiOverTwo;

                Matrix previousTransform = Transform;
                Transform = Matrix.RotationYawPitchRoll(0, 0, angle)*Matrix.Translation(p0) * Transform;
                callback(new RectangleF(-lineWidth / 2, 0, lineWidth, d));
                Transform = previousTransform;
            }
        }

        public void DrawPolyline(IEnumerable<Vector2> points, float lineWidth, float strokeThickness)
        {
            RenderRectangle callback = (r) => DrawRectangle(r, strokeThickness);
            CreatePolyLine(points, lineWidth, callback);
        }

        public void FillPolyline(IEnumerable<Vector2> points, float lineWidth)
        {
            RenderRectangle callback = FillRectangle;
            CreatePolyLine(points, lineWidth, callback);
        }

    }
}