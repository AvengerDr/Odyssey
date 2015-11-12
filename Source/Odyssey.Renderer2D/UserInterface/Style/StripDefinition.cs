﻿#region Using Directives

using System;
using System.Collections.Generic;
using SharpDX;

#endregion

namespace Odyssey.UserInterface.Style
{
    /// <summary>
    ///     Represent the definition of a grid strip.
    /// </summary>
    public class StripDefinition
    {
        private float maximumSize;
        private float minimumSize;
        private float sizeValue;
        private StripType type;

        /// <summary>
        ///     Creates a 1-Star sized strip definition.
        /// </summary>
        public StripDefinition() : this(StripType.Star, 1)
        {
        }

        /// <summary>
        ///     Creates a <see cref="StripDefinition" /> with the provided size and type.
        /// </summary>
        /// <param name="type">The type of the strip to create</param>
        /// <param name="sizeValue">The value of the strip to create</param>
        public StripDefinition(StripType type, float sizeValue = 1)
        {
            Type = type;
            SizeValue = sizeValue;
            MaximumSize = float.PositiveInfinity;
            MinimumSize = 0;
        }

        /// <summary>
        ///     The actual size of the strip in units.
        /// </summary>
        public float ActualSize { get; internal set; }

        /// <summary>
        ///     The maximum size of the strip in units.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The provided value is negative.</exception>
        /// <exception cref="InvalidOperationException">The provided value is smaller than <see cref="MinimumSize" /></exception>
        public float MaximumSize
        {
            get { return maximumSize; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value < MinimumSize)
                    throw new InvalidOperationException("The provided maximum value is smaller than the current minimum value");

                maximumSize = value;

                if (DefinitionChanged != null)
                    DefinitionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     The minimum size of the strip in units.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The provided value is negative or infinity.</exception>
        /// <exception cref="InvalidOperationException">The provided value is bigger than <see cref="MaximumSize" /></exception>
        public float MinimumSize
        {
            get { return minimumSize; }
            set
            {
                if (value < 0 || float.IsPositiveInfinity(value))
                    throw new ArgumentOutOfRangeException("value");

                if (value > MaximumSize)
                    throw new InvalidOperationException("The provided minimum value is bigger than the current maximum value");

                minimumSize = value;

                if (DefinitionChanged != null)
                    DefinitionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the type of the strip.
        /// </summary>
        public StripType Type
        {
            get { return type; }
            set
            {
                if (type == value)
                    return;

                type = value;

                var handler = DefinitionChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the size value of the strip.
        ///     Note that the value is interpreted differently depending on the strip <see cref="Type" />.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The size must be finite positive value.</exception>
        public float SizeValue
        {
            get { return sizeValue; }
            set
            {
                if (value < 0 || float.IsPositiveInfinity(value))
                    throw new ArgumentOutOfRangeException("value");

                sizeValue = value;

                var handler = DefinitionChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Clamp the provided size by the definition's minimum and maximum values.
        /// </summary>
        /// <param name="desiredSize">The size to clamp</param>
        /// <returns>The size clamped by the minimum and maximum values of the strip definition</returns>
        public float ClampSizeByMinimumMaximum(float desiredSize)
        {
            return Math.Min(MaximumSize, Math.Max(MinimumSize, desiredSize));
        }

        internal float ValueRelativeMinimum()
        {
            if (sizeValue < MathUtil.ZeroTolerance)
                return 0;

            return MinimumSize/SizeValue;
        }

        internal float ValueRelativeMaximum()
        {
            if (sizeValue < MathUtil.ZeroTolerance)
                return 0;

            return MaximumSize/SizeValue;
        }

        internal event EventHandler<EventArgs> DefinitionChanged;

        internal class SortByIncreasingStarRelativeMaximumValues : IComparer<StripDefinition>
        {
            public int Compare(StripDefinition def1, StripDefinition def2)
            {
                var val1 = def1.ValueRelativeMaximum();
                var val2 = def2.ValueRelativeMaximum();

                return val1.CompareTo(val2);
            }
        }

        internal class SortByIncreasingStarRelativeMinimumValues : IComparer<StripDefinition>
        {
            public int Compare(StripDefinition def1, StripDefinition def2)
            {
                var val1 = def1.ValueRelativeMinimum();
                var val2 = def2.ValueRelativeMinimum();

                return val1.CompareTo(val2);
            }
        }
    }
}