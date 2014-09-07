using Odyssey.Graphics.Organization;
using Odyssey.Graphics.Shaders;

namespace Odyssey.Organization.Commands
{
    public interface ITechniqueRenderCommand : IRenderCommand
    {
        Technique Technique { get; }
    }
}
