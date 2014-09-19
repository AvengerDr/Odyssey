using System;

namespace Odyssey.UserInterface.Controls
{
    public abstract class ContentControl : Control, IContentControl
    {
        private UIElement content;

        protected ContentControl(string controlStyleClass, string textStyleClass)
            : base(controlStyleClass, textStyleClass)
        {
        }

        public UIElement Content
        {
            get { return content; }
            set
            {
                content = ToDispose(value);
                content.Parent = this;
                if (Overlay != null)
                    content.Overlay = Overlay;
            }
        }

        public override bool DesignMode
        {
            get
            {
                return base.DesignMode;
            }

            protected internal set
            {
                base.DesignMode = value;
                if (Content != null)
                    Content.DesignMode = value;
            }
        }

        public override UIElement Parent
        {
            get { return base.Parent; }
            internal set
            {
                base.Parent = value;
                if (base.Parent.Overlay != null && Content != null)
                    Content.Overlay = base.Parent.Overlay;
            }
        }

        public override void Render()
        {
            base.Render();
            if (Content != null)
                Content.Render();
        }

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            if (Content != null)
                Content.Initialize();
        }

        protected override void OnLayoutUpdated(EventArgs e)
        {
            base.OnLayoutUpdated(e);
            if (Content != null)
                Content.Layout();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Content != null)
            {
                Content.Width = Width;
                Content.Height = Height;
            }
        }
    }
}