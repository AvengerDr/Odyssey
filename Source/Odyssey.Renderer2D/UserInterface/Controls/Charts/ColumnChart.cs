﻿using System.Collections;
using Odyssey.Graphics.Drawing;
using Odyssey.UserInterface.Data;
using Odyssey.UserInterface.Style;
using Odyssey.Utilities.Reflection;

namespace Odyssey.UserInterface.Controls.Charts
{
    public class ColumnChart : Chart
    {
        public ColumnChart()
            : base("Panel")
        {
        }

        public ColumnChart(string controlStyleClass, string textStyleClass) : base(controlStyleClass, textStyleClass)
        { }

        protected override DataTemplate CreateDefaultTemplate()
        {
            string typeName = GetType().Name;
            DataTemplate = new DataTemplate
            {
                Key = string.Format("{0}.TemplateInternal", typeName),
                DataType = GetType(),
                VisualTree = new ColumnItem()
                {
                    Name = typeof(ColumnItem).Name,
                    Width = Width / ((ICollection)ItemsSource).Count,
                    Height = 0,
                    Margin = new Thickness(0, 0, 4, 0),
                }
            };
            DataTemplate.Bindings.Add(ReflectionHelper.GetPropertyName((ChartItem c) => c.Value), new Binding(DataTemplate.VisualTree.Name, string.Empty));
            return DataTemplate;
        }
    }
}
