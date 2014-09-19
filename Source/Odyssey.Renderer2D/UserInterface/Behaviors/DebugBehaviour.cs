using System.Text;
using Odyssey.Interaction;
using Odyssey.Utilities.Logging;

namespace Odyssey.UserInterface.Behaviors
{
    public class DebugBehaviour : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedElement.Tap+=OnTap;
        }

        void OnTap(object sender, PointerEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} '{1}'", AssociatedElement.GetType(), AssociatedElement.Name));
            sb.AppendLine(string.Format("\tPosition: ({0}, {1})", AssociatedElement.Position.X, AssociatedElement.Position.Y));
            LogEvent.UserInterface.Info(sb.ToString());
        }

    }
}
