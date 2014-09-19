using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.UserInterface.Data;
using Odyssey.Utilities.Reflection;
using SharpDX;

namespace Odyssey.UserInterface.Controls
{
    public class RichTextBox : ItemsControl
    {
        private int lineHeight;

        public RichTextBox() : this(typeof(RichTextBox).Name) { }

        protected RichTextBox(string controlStyleClass, string textStyleClass = UserInterface.Style.TextStyle.Default)
            : base(controlStyleClass, textStyleClass)
        { }

        public IEnumerable<AdvancedLabel> Blocks { get { return Controls.OfType<AdvancedLabel>(); } }

        public int LineHeight
        {
            get { return lineHeight; }
            set
            {
                if (lineHeight == value)
                    return;
                lineHeight = value;
                foreach (var block in Blocks)
                {
                    block.Height = lineHeight;
                }
            }
        }

        protected override DataTemplate CreateDefaultTemplate()
        {
            string typeName = GetType().Name;
            DataTemplate = new DataTemplate
            {
                Key = string.Format("{0}.TemplateInternal", typeName),
                DataType = GetType(),
                VisualTree = new AdvancedLabel()
                {
                    Name = string.Format("{0}Label", typeName),
                    Width = Width - Padding.Horizontal,
                    Height = LineHeight,
                    TextStyleClass = TextStyleClass
                }
            };
            DataTemplate.Bindings.Add(ReflectionHelper.GetPropertyName((AdvancedLabel l) => l.Text), new Binding(DataTemplate.VisualTree.Name, string.Empty));
            return DataTemplate;
        }

        protected override void OnTextStyleChanged(EventArgs e)
        {
            base.OnTextStyleChanged(e);
            if (lineHeight == 0)
                LineHeight = TextStyle.Size;
            if (DataTemplate != null)
                ((Control)DataTemplate.VisualTree).TextStyleClass = TextStyleClass;
        }

        protected internal override void Arrange()
        {
            base.Arrange();
            if (!Controls.IsEmpty)
                UserInterface.Style.Layout.UpdateLayoutVertical(this, Controls);
        }
    }
}
