#region Using Directives

using System;
using Odyssey.Interaction;
using System.Linq;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public abstract class ButtonBase : ContentControl
    {
        protected const string ControlTag = "Button";

        protected ButtonBase()
            : this(ControlTag)
        {
        }

        protected ButtonBase(string controlStyleClass, string textStyleClass = UserInterface.Style.TextStyle.Default)
            : base(controlStyleClass, textStyleClass)
        {
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            ActiveStatus = ControlStatus.Enabled;
        }

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            if (Content == null)
            {
                Content = new Label() { Text = Name };
                Content.Initialize();
            }
            ToDispose(Content);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            ActiveStatus = ControlStatus.Highlighted;
        }

        public override void Render()
        {
            base.Render();
            if (Content != null)
                Content.Render();
        }

    }
}