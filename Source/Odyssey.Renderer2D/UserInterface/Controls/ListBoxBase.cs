using Odyssey.Utilities.Logging;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey.UserInterface.Controls
{
    public abstract class ListBoxBase : ItemsControl, IContainer
    {
        private const string ControlTag = "ListBox";

        protected ListBoxBase()
            : base("Panel")
        {
        }

        protected internal override void Arrange()
        {
            if (Controls.IsEmpty)
                return;

            UserInterface.Style.Layout.UpdateLayoutHorizontal(this, Controls);
        }
    }
}