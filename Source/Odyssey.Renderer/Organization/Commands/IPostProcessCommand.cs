using System.Collections.Generic;
using Odyssey.Graphics;

namespace Odyssey.Organization.Commands
{
    public interface IPostProcessCommand
    {
        List<Texture> Inputs { get; }
        Texture Output { get; set; }
        void SetInputs(IEnumerable<Texture> textures);
    }
}