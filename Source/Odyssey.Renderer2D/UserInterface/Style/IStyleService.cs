using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Graphics;
using SharpDX.DirectWrite;

namespace Odyssey.UserInterface.Style
{
    public interface IStyleService : IResourceProvider
    {
        TextDescription GetTextStyle(string themeName, string id);
        FontCollection FontCollection { get; }
        Theme GetTheme(string themeName);
        void AddResource(IResource resource);
        Brush CreateColorResource(Direct2DDevice device, ColorResource colorResource);
    }
}