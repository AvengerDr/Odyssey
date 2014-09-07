using Odyssey.Interaction;

namespace Odyssey.UserInterface.Controls
{
    public interface IOverlay
    {
        void ProcessPointerRelease(PointerEventArgs e);
        void ProcessPointerPress(PointerEventArgs e);
        void ProcessPointerMovement(PointerEventArgs e);
    }
}
