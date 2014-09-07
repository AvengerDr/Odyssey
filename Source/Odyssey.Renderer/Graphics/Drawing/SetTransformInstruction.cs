using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class SetTransformInstruction : IDesignerInstruction
    {
        private readonly Matrix transform;
        public Matrix Transform
        {
            get { return transform; }
        }

        public SetTransformInstruction(Matrix transform)
        {
            this.transform = transform == default(Matrix) ? Matrix.Identity : transform;
        }

        public void Execute(Designer designer)
        {
            designer.Transform = Transform;
        }
    }
}
