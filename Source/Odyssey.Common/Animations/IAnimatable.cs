using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Content;

namespace Odyssey.Animations
{
    public interface IAnimatable : IResource
    {
        void Play();
        void Play(string animationName);
    }

}
