using Odyssey.Interaction;

namespace Odyssey.UserInterface.Controls
{
    public interface IDesktopOverlay
    {
        void ProcessKeyDown(KeyEventArgs e);
        void ProcessKeyUp(KeyEventArgs e);
    }
}
