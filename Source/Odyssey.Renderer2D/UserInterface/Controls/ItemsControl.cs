#region Using Directives

using System;
using Odyssey.UserInterface.Data;
using Odyssey.Utilities.Logging;
using Odyssey.Utilities.Reflection;
using SharpDX;
using System.Collections;
using System.Linq;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public abstract class ItemsControl : ContainerControl
    {
        private IEnumerable itemsSource;

        protected ItemsControl(string styleClass)
            : base(styleClass)
        {
        }

        public DataTemplate DataTemplate { get; set; }

        public IEnumerable ItemsSource
        {
            get { return itemsSource; }
            set
            {
                if (itemsSource != value)
                {
                    itemsSource = value;
                    DataContext = value;
                }
            }
        }

        protected virtual DataTemplate CreateDefaultTemplate()
        {
            string typeName = GetType().Name;
            DataTemplate = new DataTemplate
            {
                Key = string.Format("{0}.TemplateInternal", typeName),
                DataType = GetType(),
                VisualTree = new Label
                {
                    Name = string.Format("{0}Label", typeName),
                    TextStyleClass = TextStyleClass
                }
            };
            DataTemplate.Bindings.Add(ReflectionHelper.GetPropertyName((Label l) => l.Text), new Binding(DataTemplate.VisualTree.Name, string.Empty));
            return DataTemplate;
        }

        protected override void OnInitializing(ControlEventArgs e)
        {
            base.OnInitializing(e);
            if (DataTemplate == null)
                DataTemplate = CreateDefaultTemplate();
            if (DataTemplate.DataType == null)
                throw new InvalidOperationException("No DataType specified");
            if (!ReflectionHelper.IsTypeDerived(DataTemplate.DataType, GetType()))
            {
                LogEvent.UserInterface.Warning(string.Format("DataTemplate '{0}' expects type {1}. Element '{2}' is of type {3}",
                    DataTemplate.Key, DataTemplate.DataType.Name, Name, GetType().Name));
                return;
            }

            foreach (var item in ItemsSource)
            {
                UIElement previousElement = this;
                foreach (UIElement element in TreeTraversal.PreOrderVisit(DataTemplate.VisualTree))
                {
                    UIElement newItem = element.Copy();
                    
                    var contentControlParent = element.Parent as ContentControl;
                    if (contentControlParent != null)
                        ((ContentControl)previousElement).Content = newItem;
                    else
                        Add(ToDispose(newItem));

                    newItem.DataContext = item;

                    foreach (var kvp in DataTemplate.Bindings.Where(kvp => string.Equals(newItem.Name, kvp.Value.TargetElement)))
                    {
                        newItem.SetBinding(kvp.Value, kvp.Key);
                    }
                    
                    previousElement = newItem;
                }

                //TODO Implement Binding
            }
        }

        #region IContainer members

        public override void Render()
        {
            foreach (UIElement control in Controls.Where(control => control.IsVisible))
                control.Render();
        }

        #endregion IContainer members
    }
}