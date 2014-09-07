using System.Collections.Generic;
using Odyssey.UserInterface.Style;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class FigureDesigner
    {
        private readonly GeometrySink sink;
        private Vector2 startPoint;
        private Vector2 previousPoint;
        private bool isFigureOpened;

        public FigureDesigner(GeometrySink sink)
        {
            this.sink = sink;
        }

        public void Execute(IEnumerable<VectorCommand> commands)
        {
            foreach (var instruction in commands)
            {
                switch (instruction.Command)
                {
                    case 'M':
                        Move(sink, instruction, true);
                        break;

                    case 'L':
                        Line(sink, instruction,true);
                        break;

                    case 'Z':
                        Close(sink, FigureEnd.Closed);
                        break;
                }
            }
        }

        void Move(GeometrySink sink, VectorCommand instruction, bool isAbsolute)
        {
            if (isFigureOpened)
                Close(sink, FigureEnd.Open);

            Vector2 point = new Vector2(instruction.Arguments[0], instruction.Arguments[1]);
            startPoint = isAbsolute ? point : point + startPoint;
            previousPoint = startPoint;
            sink.BeginFigure(startPoint, FigureBegin.Filled);
        }

        void Line(GeometrySink sink, VectorCommand instruction, bool isAbsolute)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < instruction.Arguments.Length; i = i + 2)
            {
                Vector2 point = new Vector2(instruction.Arguments[i], instruction.Arguments[i+1]);
                if (isAbsolute)
                {
                    points.Add(point);
                }
                else
                {
                    points.Add(point + previousPoint);
                }
                previousPoint = points[i];
            }
            sink.AddLines(points);
        }

        void Close(GeometrySink sink, FigureEnd endType)
        {
            sink.EndFigure(endType);
        }

    }
}
