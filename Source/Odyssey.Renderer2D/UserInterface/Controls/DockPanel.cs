﻿using System;
using Odyssey.Core;
using SharpDX.Mathematics;

namespace Odyssey.UserInterface.Controls
{
    public enum Dock
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public class DockPanel : PanelBase
    {
        /// <summary>
        /// The key to the Dock attached dependency property. This defines the position of a child item within the panel.
        /// </summary>
        public readonly static PropertyKey<Dock> DockPropertyKey = new PropertyKey<Dock>("DockKey", typeof(DockPanel), DefaultValueMetadata.Static(Dock.Left));

        public DockPanel(string controlClass) : base(controlClass)
        {
            LastChildFill = true;
        }

        public bool LastChildFill { get; set; }

        public DockPanel(): this("Empty")
        {
        }

        protected override Vector2 MeasureOverride(Vector2 availableSizeWithoutMargins)
        {
            float availableWidth = availableSizeWithoutMargins.X;
            float availableHeight = availableSizeWithoutMargins.Y;

            for (int i = 0; i < Controls.Count; i++)
            {
                var control = Controls[i];

                Dock dock = control.DependencyProperties.Get(DockPropertyKey);
                switch (dock)
                {
                    case Dock.Bottom:
                        control.Measure(new Vector2(availableWidth, control.Height));
                        availableHeight -= control.DesiredSize.Y;
                        break;

                    case Dock.Right:
                        control.Measure(new Vector2(control.Width, availableHeight));
                        availableWidth -= control.DesiredSize.X;
                        break;

                    case Dock.Left:
                        control.Measure(new Vector2(control.Width, availableHeight));
                        availableWidth -= control.DesiredSize.X;
                        break;

                    case Dock.Top:
                        control.Measure(new Vector2(availableWidth, control.Height));
                        availableHeight -= control.DesiredSize.Y;
                        break;
                }
            }

            return availableSizeWithoutMargins;
        }

        protected override Vector2 ArrangeOverride(Vector2 availableSizeWithoutMargins)
        {
            float availableWidth = availableSizeWithoutMargins.X;
            float availableHeight = availableSizeWithoutMargins.Y;
            float topOffset=0;
            float rightOffset=0;
            float leftOffset=0;
            float bottomOffset=0;

            for (int i = 0; i < Controls.Count; i++)
            {
                var control = Controls[i];

                if (LastChildFill && i == Controls.Count - 1)
                {
                    control.Position = new Vector2(leftOffset, topOffset);
                    control.Width = availableWidth;
                    control.Height = availableHeight;
                    control.Arrange(new Vector2(availableWidth, availableHeight));
                }
                else
                {
                    Dock dock = control.DependencyProperties.Get(DockPropertyKey);
                    switch (dock)
                    {
                        case Dock.Bottom:
                            control.Position = new Vector2(leftOffset, availableHeight - control.DesiredSizeWithMargins.Y - bottomOffset);
                            control.Arrange(new Vector2(availableWidth, control.DesiredSizeWithMargins.Y));
                            bottomOffset += control.DesiredSizeWithMargins.Y;
                            availableHeight -= control.DesiredSizeWithMargins.Y;
                            break;

                        case Dock.Right:
                            control.Position = new Vector2(availableWidth - control.DesiredSizeWithMargins.X - rightOffset, topOffset);
                            control.Arrange(new Vector2(control.DesiredSizeWithMargins.X, availableHeight));
                            rightOffset += control.DesiredSizeWithMargins.X;
                            availableWidth -= control.DesiredSizeWithMargins.X;
                            break;

                        case Dock.Left:
                            control.Position = new Vector2(leftOffset, topOffset);
                            control.Arrange(new Vector2(control.DesiredSizeWithMargins.X, availableHeight));
                            leftOffset += control.DesiredSizeWithMargins.X;
                            availableWidth -= control.DesiredSizeWithMargins.X;
                            break;

                        case Dock.Top:
                            control.Position = new Vector2(leftOffset, topOffset);
                            control.Arrange(new Vector2(availableWidth, control.DesiredSizeWithMargins.Y));
                            topOffset += control.DesiredSize.Y;
                            availableHeight -= control.DesiredSize.Y;
                            break;
                    }
                }
            }

            return availableSizeWithoutMargins;
        }

    }
}