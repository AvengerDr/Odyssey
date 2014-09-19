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

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SharpDX;

#endregion

namespace Odyssey.UserInterface.Controls
{
    public abstract class ContainerControl : Control, IContainer
    {
        protected ContainerControl(string controlStyleClass, string textStyleClass = UserInterface.Style.TextStyle.Default) : base(controlStyleClass, textStyleClass)
        {
            Controls = new ControlCollection(this);
            IsFocusable = false;
        }

        /// <summary>
        /// Returns the publicly available collection of child controls.
        /// </summary>
        public virtual ControlCollection Controls { get; private set; }

        #region IContainer Members

        /// <summary>
        /// Occurs when a new control is added to the <see cref="ControlCollection"/>.
        /// </summary>
        public event EventHandler<EventArgs> ControlAdded;

        public override bool DesignMode
        {
            get { return base.DesignMode; }
            protected internal set
            {
                base.DesignMode = value;
                foreach (UIElement childControl in Controls)
                    childControl.DesignMode = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="ControlAdded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ControlEventArgs"/> instance
        /// containing the event data.</param>
        protected virtual void OnControlAdded(EventArgs e)
        {
            RaiseEvent(ControlAdded, this, e);
            if (!DesignMode)
                Layout();
        }

        #endregion IContainer Members

        public override UIElement Parent
        {
            get { return base.Parent; }
            internal set
            {
                if (base.Parent == value) return;

                base.Parent = value;
                foreach (UIElement control in Controls)
                    control.Parent = this;
            }
        }

        public void Add(UIElement control)
        {
            control.Parent = this;
            control.Index = Controls.Count;
            Controls.Add(ToDispose(control));
            OnControlAdded(new EventArgs());
        }

        public virtual void AddRange(IEnumerable<UIElement> controls)
        {
            foreach (UIElement ctl in controls)
                Add(ctl);
        }

        /// <summary>
        /// Determines whether the <b>ContainerControl</b> contains the specified Key.
        /// </summary>
        /// <param name="control">The control to locate in the control collection.</param>
        /// <returns><b>True</b> if it the collection contains that element ,<b>false</b>
        /// otherwise.</returns>
        /// <remarks>
        /// The control passed as parameter does not have to be a top level child, but this method
        /// will also return true if the specified <see cref="UIElement"/> belongs to the tree
        /// formed by the ContainerControl's children.
        /// </remarks>
        public bool ContainsControl(UIElement control)
        {
            Contract.Requires<ArgumentNullException>(control != null, "control is null");

            return Controls.Contains(control);
        }

        public bool ContainsControl(string name)
        {
            return Controls.Any(c => string.Equals(c.Name, name));
        }

        public UIElement Find(string id)
        {
            foreach (UIElement ctl in TreeTraversal.PreOrderVisit(this).Skip(1))
            {
                if (ctl.Name == id)
                    return ctl;
                else
                    continue;
            }
            return null;
        }

        public UIElement Find(Vector2 cursorLocation)
        {
            return TreeTraversal.PostOrderInteractionVisit(this)
                .Reverse()
                .FirstOrDefault(control => control.Contains(cursorLocation));
        }

        public void Insert(int index, UIElement control)
        {
            Controls.Insert(index, control);
        }

        public void Remove(UIElement control)
        {
            Controls.Remove(control);
        }

        public override void Render()
        {
            base.Render();
            foreach (var control in Controls.Where(control => control.IsVisible))
                control.Render();
        }

        protected internal override UIElement Copy()
        {
            UIElement copy = base.Copy();
            CopyEvents(typeof(ContainerControl), this, copy);
            IContainer containerCopy = (IContainer) copy;
            foreach (UIElement child in Controls)
                containerCopy.Controls.Add(child.Copy());

            return copy;
        }

        public override void Layout()
        {
            base.Layout();
            foreach (UIElement ctl in Controls)
            {
                ctl.Layout();
            }
        }

        //protected internal override void Measure()
        //{
        //    base.Measure();
        //    foreach (UIElement ctl in Controls)
        //    {
        //        ctl.Measure();
        //    }
        //}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            foreach (UIElement element in Controls)
            {
                element.Initialize();
            }
        }

        #region Debug

#if DEBUG
        internal static void Debug(IEnumerable<UIElement> iterator)
        {
            System.Diagnostics.Debug.WriteLine("---------");
            foreach (UIElement ctl in iterator)
            {
                System.Diagnostics.Debug.WriteLine(ctl.Name);
            }
            System.Diagnostics.Debug.WriteLine("---------");
        }
#endif

        #endregion Debug
    }
}