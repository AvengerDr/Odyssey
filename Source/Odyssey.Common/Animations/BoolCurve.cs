using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.Animations
{
    public class BoolCurve : AnimationCurve<BoolKeyFrame>
    {
        public BoolCurve()
        {
            Function = Discrete;
        }
    }
}
