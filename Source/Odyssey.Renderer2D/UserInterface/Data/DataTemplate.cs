using System;
using System.Collections.Generic;
using System.Linq;

namespace Odyssey.UserInterface.Data
{
    public class DataTemplate
    {
        private readonly BindingCollection bindings;

        public DataTemplate()
        {
            bindings = new BindingCollection();
        }

        public BindingCollection Bindings
        {
            get
            {
                return bindings;
            }
        }

        public Type DataType { get; set; }

        public string Key { get; set; }

        public UIElement VisualTree { get; set; }
    }
}