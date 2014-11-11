﻿using System.Collections;
using System.Collections.Generic;
using Odyssey.Core;

namespace Odyssey.Collections
{
    /// <summary>
    /// A generic component collection.
    /// </summary>
    /// <typeparam name="T">Type of the collection</typeparam>
    public abstract class ComponentCollection<T> : IEnumerable<T> where T : ComponentBase
    {
        internal protected readonly List<T> Items;
        private readonly Dictionary<string, T> mapItems;

        protected ComponentCollection()
        {
            Items = new List<T>();
            mapItems = new Dictionary<string, T>();
        }

        protected ComponentCollection(int capacity)
        {
            Items = new List<T>(capacity);
            mapItems = new Dictionary<string, T>(capacity);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal protected T Add(T item)
        {
            Items.Add(item);
            mapItems.Add(item.Name, item);
            return item;
        }

        internal protected void Clear()
        {
            Items.Clear();
            mapItems.Clear();
        }

        protected int Capacity
        {
            get
            {
                return Items.Capacity;
            }
            set
            {
                Items.Capacity = value;
            }
        }

        /// <summary>
        /// Gets the number of objects in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        /// <summary>Gets a specific element in the collection by using an index value.</summary>
        /// <param name="index">Index of the EffectTechnique to get.</param>
        public T this[int index]
        {
            get
            {
                if ((index >= 0) && (index < Items.Count))
                {
                    return Items[index];
                }
                return null;
            }
        }

        /// <summary>Gets a specific element in the collection by using a name.</summary>
        /// <param name="name">Name of the EffectTechnique to get.</param>
        public T this[string name]
        {
            get
            {
                T value;
                if (!mapItems.TryGetValue(name, out value))
                {
                    value = TryToGetOnNotFound(name);
                }
                return value;
            }
        }

        /// <summary>
        /// Determines whether this collection contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [contains] an element with the specified name; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return mapItems.ContainsKey(name);
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual T TryToGetOnNotFound(string name)
        {
            return null;
        }

        #endregion
    }    
}
