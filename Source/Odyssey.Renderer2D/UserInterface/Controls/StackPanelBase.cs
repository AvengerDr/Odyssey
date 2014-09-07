#region Using Directives

using System;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public abstract class StackPanelBase : ItemsControl
    {
        private const string ControlTag = "StackPanel";
        private Orientation orientation;

        protected StackPanelBase()
            : base("Panel")
        {
        }

        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    OnLayoutUpdated(EventArgs.Empty);
                }
            }
        }

        protected internal override void Arrange()
        {
            if (Controls.IsEmpty)
                return;

            if (Orientation == Orientation.Horizontal)
                UserInterface.Style.Layout.UpdateLayoutHorizontal(this, Controls);
            else
                UserInterface.Style.Layout.UpdateLayoutVertical(this, Controls);
        }
    }
}