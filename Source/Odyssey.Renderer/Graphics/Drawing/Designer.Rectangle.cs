using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Geometry;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public partial class Designer
    {
        private delegate Color4[] RectangleColorShader(IColorResource color, int widthSegments, int heightSegments);

        public void DrawRectangle(RectangleF rectangle, float strokeThickness)
        {
            Contract.Requires<ArgumentException>(!rectangle.IsEmpty, "rectangle");
            var outline = new[]
            {
                // Top
                new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, strokeThickness),
                // Bottom 
                new RectangleF(rectangle.X, rectangle.Y + rectangle.Height - strokeThickness, rectangle.Width, strokeThickness),
                // Left
                new RectangleF(rectangle.X, rectangle.Y+strokeThickness, strokeThickness, rectangle.Height - 2 * strokeThickness),
                // Right
                new RectangleF(rectangle.X + rectangle.Width - strokeThickness, rectangle.Y+strokeThickness, strokeThickness, rectangle.Height - 2*strokeThickness)
            };

            foreach (var r in outline)
            {
                FillRectangle(r);
            }

        }

        public void FillRectangle(RectangleF rectangle)
        {
            Contract.Requires<ArgumentException>(!rectangle.IsEmpty, "rectangle");
            Contract.Requires<ArgumentException>(Color != null, "color");

            float[] widthSegments;
            float[] heightSegments;

            RectangleColorShader shader = ChooseShader(Color, out widthSegments, out heightSegments);

            int[] indices;
            Vector3[] vertices = CreateRectangleMesh(rectangle, widthSegments, heightSegments, Transform, out indices);
            Color4[] colors = shader(Color, widthSegments.Length, heightSegments.Length);

            var vertexArray = new VertexPositionColor[vertices.Length];
            for (int i=0; i<vertices.Length;i++)
                vertexArray[i] = new VertexPositionColor() { Position = vertices[i], Color = colors[i] };

            shapes.Add(new ShapeMeshDescription() { Vertices = vertexArray, Indices = indices });
        }

        static Color4[] RectangleVertical(IColorResource color, int widthSegments, int heightSegments)
        {
            var gradient = (IGradient) color;
            Color4[] colors = new Color4[widthSegments* heightSegments];
            int index = colors.Length-1;
            for (int i = 0; i < heightSegments; i++)
            {
                for (int j = 0; j < widthSegments; j++)
                {
                    colors[index--] = gradient.GradientStops[i].Color;
                }
            }
            return colors;
        }

        static Color4[] RectangleHorizontal(IColorResource color, int widthSegments, int heightSegments)
        {
            var gradient = (IGradient)color;
            Color4[] colors = new Color4[widthSegments * heightSegments];
            int index = colors.Length - 1; ;
            for (int i = 0; i < heightSegments; i++)
            {
                for (int j = 0; j < widthSegments; j++)
                {
                    colors[index--] = gradient.GradientStops[j].Color;
                }
            }
            return colors;
        }

        static Color4[] RectangleUniform(IColorResource color, int widthSegments, int heightSegments)
        {
            var solidColor = (SolidColor)color;
            Color4[] colors = new Color4[widthSegments * heightSegments];
            int index = colors.Length - 1; ;
            for (int i = 0; i < heightSegments; i++)
            {
                for (int j = 0; j < widthSegments; j++)
                {
                    colors[index--] = solidColor.Color;
                }
            }
            return colors;
        }

        private static Vector3[] CreateRectangleMesh(RectangleF rectangle, float[] widthSegments, float[] heightSegments, Matrix transform, out int[] indices)
        {
            Contract.Requires<ArgumentException>(widthSegments != null);
            Contract.Requires<ArgumentException>(heightSegments != null);
            Contract.Requires<ArgumentException>(widthSegments.Length >= 2);
            Contract.Requires<ArgumentException>(heightSegments.Length >= 2);

            int rows = heightSegments.Length - 1;
            int columns = widthSegments.Length - 1;

            Vector3[] vertices = new Vector3[(1 + rows) * (1 + columns)];
            indices = new int[rows * columns * 6];

            int vertexCount = 0, indexCount = 0;
            float x = rectangle.X;
            float y = rectangle.Y;
            const float z = 0;
            float width = rectangle.Width;
            float height = rectangle.Height;

            // Compute vertices, one row at a time
            for (int i = 0; i < rows + 1; i++)
            {
                for (int j = 0; j < columns + 1; j++)
                {
                    float hOffset = widthSegments[j] * width;
                    float vOffset = heightSegments[i] * height;

                    vertices[vertexCount] = new Vector3(x + hOffset, y + vOffset, z);

                    if (i < rows && j < columns)
                    {
                        indices[indexCount] = vertexCount + columns + 1 ;
                        indices[indexCount + 1] = vertexCount + 1;
                        indices[indexCount + 2] = vertexCount;

                        indices[indexCount + 3] = indices[indexCount];
                        indices[indexCount + 4] = indices[indexCount] + 1;
                        indices[indexCount + 5] = indices[indexCount + 1];
                        indexCount += 6;
                    }

                    vertexCount++;
                }
            }

            if (!transform.IsIdentity)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    var vertex = vertices[i];
                    vertices[i] = Vector3.Transform(vertex, transform).ToVector3();
                }
            }
            return vertices;
        }

        static RectangleColorShader ChooseShader(IColorResource color, out float[] widthSegments, out float[] heightSegments)
        {
            RectangleColorShader shader;
            switch (color.Type)
            {
                default:
                case ColorType.SolidColor:
                    widthSegments = new[] {0f, 1f};
                    heightSegments = new[] { 0f, 1f };
                    shader = RectangleUniform;
                    break;

                case ColorType.LinearGradient:
                    var gLinear = (LinearGradient)color;
                    Vector2 direction = gLinear.EndPoint - gLinear.StartPoint;
                    if (direction == Vector2.UnitY)
                    {
                        widthSegments = new[] {0f, 1f};
                        heightSegments = gLinear.GradientStops.Select(gs => gs.Offset).ToArray();
                        shader = RectangleVertical;
                    }
                    else if (direction == Vector2.UnitX)
                    {
                        widthSegments = gLinear.GradientStops.Select(gs => gs.Offset).ToArray();
                        heightSegments = new[] { 0f, 1f };
                        shader = RectangleHorizontal;
                    }
                    else
                        throw new NotSupportedException("Non axis-aligned gradient are not supported");
                    break;

                case ColorType.RadialGradient:
                    throw new NotSupportedException("Rectangle does not support radial gradient");
            }
            return shader;
        }
    }
}
