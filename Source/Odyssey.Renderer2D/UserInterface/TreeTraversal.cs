#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
//
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
//
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion License

#region Using Directives

using Odyssey.UserInterface.Controls;
using System.Collections.Generic;

#endregion Using Directives

namespace Odyssey.UserInterface
{
    public static class TreeTraversal
    {
        public static IEnumerable<UIElement> PostOrderInteractionVisit(UIElement root)
        {
            IContainer containerControl = root as IContainer;
            if (containerControl != null)
                foreach (UIElement control in containerControl.Controls.InteractionEnabledControls)
                    foreach (UIElement ctlChild in PostOrderInteractionVisit(control))
                        yield return ctlChild;
            else
            {
                var contentControl = root as IContentControl;
                if (contentControl != null && contentControl.Content != null)
                    yield return contentControl.Content;
            }
            yield return root;
        }

        public static IEnumerable<UIElement> PreOrderVisit(UIElement root)
        {
            yield return root;
            IContainer containerControl = root as IContainer;
            if (containerControl != null)
                foreach (UIElement control in containerControl.Controls)
                    foreach (UIElement ctlChild in PreOrderVisit(control))
                        yield return ctlChild;
            else
            {
                var contentControl = root as IContentControl;
                if (contentControl != null && contentControl.Content != null)
                    yield return contentControl.Content;
            }
        }
    }
}