﻿using System;
using System.Linq;
using Odyssey.Geometry;
using Odyssey.Geometry.Primitives;
using Odyssey.Graphics.Models;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public partial class Designer
    {
        private delegate Color4[] EllipseColorShader(Ellipse ellipse, IColorResource color, int numVertex, int slices);

        public void FillEllipse(Ellipse ellipse, int slices = 6)
        {
            float[] offsets;
            EllipseColorShader shader = ChooseEllipseShader(Color, out offsets);
            Color4[] colors = shader(ellipse, Color, (offsets.Length - 1) * (slices) + 1, slices);
            int[] indices;
            Vector3[] vertices = EllipseMesh.CreateMesh(ellipse, offsets, slices, Transform, out indices);
            
            var vertexArray = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertexArray[i] = new VertexPositionColor() { Position = vertices[i], Color = colors[i] };

            shapes.Add(new ShapeMeshDescription() { Vertices = vertexArray, Indices = indices });
        }

        public void DrawEllipse(Ellipse ellipse, float ringWidth, int slices = 64)
        {
            float innerRadiusRatio = 1 - (ringWidth / ellipse.RadiusX);
            float[] offsets;

            var solidColor = Color as SolidColor;
            if (solidColor != null)
            {
                Color = RadialGradient.New(Color.Name,
                    new[]
                    {
                        new GradientStop(SharpDX.Color.Transparent, 0),
                        new GradientStop(solidColor.Color, innerRadiusRatio), new GradientStop(solidColor.Color, 1)
                    });
            }

            EllipseColorShader shader = ChooseEllipseOutlineShader(innerRadiusRatio, Color, out offsets);
            Color4[] colors = shader(ellipse, Color, offsets.Length * slices, slices);
            int[] indices;
            Vector3[] vertices = EllipseMesh.CreateRingMesh(ellipse, offsets, slices, Transform, out indices);

            var vertexArray = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertexArray[i] = new VertexPositionColor() { Position = vertices[i], Color = colors[i] };

            shapes.Add(new ShapeMeshDescription() { Vertices = vertexArray, Indices = indices });
        }


        #region Color Shaders
        private static Color4[] EllipseRadial(Ellipse ellipse, IColorResource color, int numVertex, int slices)
        {
            var gradient = (IGradient)color;
            Color4[] colors = new Color4[numVertex];
            colors[0] = gradient.GradientStops[0].Color;
            int k = 1;
            for (int i = 1; i < colors.Length; i++)
            {
                if (i > 1 && (i - 1) % slices == 0)
                    k++;
                colors[i] = gradient.GradientStops[k].Color;
            }

            return colors;
        }

        private static Color4[] EllipseUniform(Ellipse ellipse, IColorResource color, int numVertex, int slices)
        {
            var solidColor = (SolidColor) color;
            Color4[] colors = new Color4[numVertex];
            for (int i = 0; i < numVertex; i++)
                colors[i] = solidColor.Color;
            return colors;
        }

        private static Color4[] EllipseOutlineRadial(Ellipse ellipse, IColorResource color, int numVertex, int slices)
        {
            var gradient = (IGradient)color;
            Color4[] colors = new Color4[numVertex];
            int k = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                if (i > 0 && i % slices == 0)
                    k++;
                colors[i] = gradient.GradientStops[k].Color;
            }

            return colors;
        }

        private static Color4[] EllipseVertical(Ellipse ellipse, IColorResource color, int numVertex, int slices)
        {
            var gradient = (IGradient) color;
            // Only one ellipse ring is supported
            Color4[] colors = new Color4[numVertex];
            float verticalAxis = ellipse.VerticalAxis;

            var vertices = ellipse.CalculateVertices(slices).ToArray();

            for (int i = 0; i < vertices.Length; i++)
            {
                float r = Math.Abs(vertices[i].Y - ellipse.Center.Y - ellipse.RadiusY) / verticalAxis;
                colors[i + 1] = gradient.GradientStops.Evaluate(r);
            }

            // Color of the center vertex is equal to the color of one of the sides
            colors[0] = colors[1];

            return colors;
        }

        private static Color4[] EllipseHorizontal(Ellipse ellipse, IColorResource color, int numVertex, int slices)
        {
            var gradient = (IGradient)color;
            // Only one ellipse ring is supported
            Color4[] colors = new Color4[numVertex];
            float horizontalAxis = ellipse.HorizontalAxis;

            var vertices = ellipse.CalculateVertices(slices).ToArray();

            for (int i = 0; i < vertices.Length; i++)
            {
                float r = Math.Abs(vertices[i].X - ellipse.Center.X - ellipse.RadiusX) / horizontalAxis;
                colors[i + 1] = gradient.GradientStops.Evaluate(r);
            }

            // Color of the center vertex is equal to the color of the top or bottom vertex
            colors[0] = colors[slices / 4 + 1];

            return colors;
        } 
        #endregion

        static EllipseColorShader ChooseEllipseShader(IColorResource color, out float[] ringOffsets)
        {
            EllipseColorShader shader;
            switch (color.Type)
            {
                case ColorType.SolidColor:
                    ringOffsets = new[] {0f, 1f};
                    shader = EllipseUniform;
                    break;

                case ColorType.LinearGradient:
                    var gLinear = (LinearGradient)color;

                    if (gLinear.GradientStops.Count > 2)
                        throw new NotSupportedException("Ellipse supports only two linear gradient stops");

                    ringOffsets = gLinear.GradientStops.Select(g => g.Offset).ToArray();
                    Vector2 direction = gLinear.EndPoint -gLinear.StartPoint;
                    if (direction == Vector2.UnitY)
                        shader = EllipseVertical;
                    else if (direction == Vector2.UnitX)
                        shader = EllipseHorizontal;
                    else
                        throw new NotSupportedException("Non axis-aligned gradient are not supported");
                    break;

                case ColorType.RadialGradient:
                    ringOffsets = ((IGradient)color).GradientStops.Select(g => g.Offset).ToArray();
                    shader = EllipseRadial;
                    break;

                default:
                    throw new NotSupportedException(String.Format("Gradient `{0}' is not supported by Ellipse", color.Type));
            }
            return shader;
        }

        static EllipseColorShader ChooseEllipseOutlineShader(float innerRadiusRatio, IColorResource color, out float[] ringOffsets)
        {

            EllipseColorShader shader;
            switch (color.Type)
            {
                case ColorType.SolidColor:
                case ColorType.RadialGradient:
                    ringOffsets = ((IGradient)color).GradientStops.Select(g => (float)MathHelper.ConvertRange(0, 1, innerRadiusRatio, 1, g.Offset)).ToArray();
                    shader = EllipseOutlineRadial;
                    break;

                default:
                    throw new NotSupportedException(String.Format("Gradient `{0}' is not supported by EllipseOutline", color.Type));
            }
            return shader;
        }
    }
}
