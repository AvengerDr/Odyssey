using Odyssey.UserInterface.Style;
using SharpDX;

namespace Odyssey.UserInterface.Controls
{
    public abstract class PanelBase : ContainerControl
    {
        protected const string ControlTag = "Panel";

        #region Constructors

        protected PanelBase() : base(ControlTag)
        {
        }

        protected PanelBase(string controlClass)
            : base(controlClass)
        {}

        #endregion
    }
}