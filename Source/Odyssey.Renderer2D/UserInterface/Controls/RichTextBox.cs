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

        protected RichTextBox(string controlClass)
            : base(controlClass)
        {
        }

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

        protected override void OnTextDefinitionChanged(EventArgs e)
        {
            base.OnTextDefinitionChanged(e);
            if (lineHeight == 0)
                LineHeight = TextDescription.Size;
            if (DataTemplate != null)
                ((Control)DataTemplate.VisualTree).TextStyleClass = TextStyleClass;
        }

        public override bool Contains(Vector2 cursorLocation)
        {
            return BoundingRectangle.Contains(cursorLocation);
        }

        protected internal override void Arrange()
        {
            if (!Controls.IsEmpty)
                UserInterface.Style.Layout.UpdateLayoutVertical(this, Controls);
            base.Arrange();
        }
    }
}
