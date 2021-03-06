﻿using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.UserInterface.Style;
using SharpDX.Mathematics;

namespace Odyssey.UserInterface.Controls
{
    public class Grid : GridBase
    {
        private readonly StripDefinitionCollection[] stripDefinitions =
        {
            new StripDefinitionCollection(),
            new StripDefinitionCollection(),
            new StripDefinitionCollection(),
        };

        private readonly List<float>[] cachedStripPositions =
        {
            new List<float>(),
            new List<float>(), 
            new List<float>()
        };


        private readonly List<float>[] cachedStripSizes =
        {
            new List<float>(),
            new List<float>(), 
            new List<float>()
        };

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            if (stripDefinitions[2].Count == 0)
                AddLayerDefinition(new StripDefinition(StripType.Fixed,1));
        }

        void CalculateStripPositions()
        {
            for (int dim = 0; dim < stripDefinitions.Length; dim++)
            {
                cachedStripPositions[dim].Clear();
                float startPosition = 0;
                for (int i = 0; i < stripDefinitions[dim].Count; i++)
                {
                    var strip = stripDefinitions[dim][i];
                    cachedStripPositions[dim].Add(startPosition);
                    startPosition += strip.ActualSize;
                }
                cachedStripPositions[dim].Add(startPosition);
            }
        }

        void CalculateStripSizes()
        {
            for (int dim = 0; dim < stripDefinitions.Length; dim++)
            {
                cachedStripSizes[dim].Clear();
                for (int i = 0; i < stripDefinitions[dim].Count; i++)
                {
                    var strip = stripDefinitions[dim][i];
                    cachedStripSizes[dim].Add(strip.ActualSize);
                }
            }
        }

        protected override Vector3 MeasureOverride(Vector3 availableSizeWithoutMargins)
        {
            for (int dim = 0; dim < stripDefinitions.Length; dim++)
            {
                float stripSum = stripDefinitions[dim].Sum(s => s.SizeValue);
                for (int i = 0; i < stripDefinitions[dim].Count; i++)
                {
                    var strip = stripDefinitions[dim][i];
                    strip.ActualSize = strip.SizeValue/stripSum*availableSizeWithoutMargins[dim];
                }
            }

            CalculateStripPositions();
            CalculateStripSizes();

            foreach (var control in Children.Visual)
            {
                var gridPosition = GetElementGridPositions(control);
                control.Measure(new Vector3(cachedStripSizes[0][gridPosition.X], cachedStripSizes[1][gridPosition.Y], cachedStripSizes[2][gridPosition.Z]));
            }

            return availableSizeWithoutMargins;
        }

        protected override Vector3 ArrangeOverride(Vector3 availableSizeWithoutMargins)
        {
            CalculateStripPositions();
            CalculateStripSizes();

            foreach (var control in Children.Visual)
            {
                var gridPosition = GetElementGridPositions(control);
                control.Position = new Vector3(cachedStripPositions[0][gridPosition.X], cachedStripPositions[1][gridPosition.Y], cachedStripPositions[2][gridPosition.Z]);
                control.Arrange(new Vector3(cachedStripSizes[0][gridPosition.X], cachedStripSizes[1][gridPosition.Y], cachedStripSizes[2][gridPosition.Z]));
            }

            var finalSize = Vector3.Zero;
            for (int dim = 0; dim < 3; dim++)
                finalSize[dim] = Math.Max(cachedStripPositions[dim][stripDefinitions[dim].Count], availableSizeWithoutMargins[dim]);

            return finalSize;
        }

        public void AddRowDefinition(StripDefinition row)
        {
            stripDefinitions[1].Add(row);
        }

        public void AddColumnDefinition(StripDefinition column)
        {
            stripDefinitions[0].Add(column);
        }

        public void AddLayerDefinition(StripDefinition layer)
        {
            stripDefinitions[2].Add(layer);
        }
    }
}
