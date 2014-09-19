#region Using Directives

using System.Linq;
using Odyssey.UserInterface.Controls;
using SharpDX;
using System.Collections.Generic;

#endregion Using Directives

namespace Odyssey.UserInterface.Style
{
    public static class Layout
    {
        public static Vector2 CenterControl(UIElement control, UIElement container)
        {
            return new Vector2(CenterControlHorizontal(control, container),
                CenterControlVertical(control, container));
        }

        internal static void UpdateLayoutHorizontal(Controls.Control parent, IEnumerable<UIElement> children)
        {
            Vector2 previousPosition = parent.TopLeftPosition + new Vector2(parent.Padding.Left, parent.Padding.Top);
            foreach (UIElement element in children)
            {
                var margin = element.Margin;
                element.Position = previousPosition + new Vector2(margin.Left, margin.Top);
                previousPosition = element.Position + new Vector2(element.Width + margin.Right, -margin.Top);
            }
        }

        public static void UpdateLayoutVertical(Control parent, IEnumerable<UIElement> children)
        {
            Vector2 previousPosition = parent.TopLeftPosition + new Vector2(parent.Padding.Left, parent.Padding.Top);
            foreach (UIElement element in children)
            {
                var margin = element.Margin;
                element.Position = previousPosition + new Vector2(margin.Left, margin.Top);
                previousPosition = new Vector2(-margin.Left, element.Position.Y + element.Height + margin.Bottom);
            }
        }

        private static float CenterControlHorizontal(UIElement control, UIElement container)
        {
            return (container.Width - control.Width) / 2f;
        }

        private static float CenterControlVertical(UIElement control, UIElement container)
        {
            return (container.Height - control.Height) / 2f;
        }
    }
}