using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Odyssey.Utilities.Collections
{
    public class CollectionMap<TIndex, TCollection, TItem> : IEnumerable<TCollection> 
        where TCollection : ICollection<TItem>, new()
    {
        readonly Dictionary<TIndex, TCollection> collectionMap;

        public CollectionMap()
        {
            collectionMap = new Dictionary<TIndex,TCollection>();
        }

        public bool ContainsKey(TIndex index)
        {
            Contract.Requires<ArgumentNullException>(index != null, "index");
            return collectionMap.ContainsKey(index);
        }

        public bool ContainsItem(TIndex index, TItem item)
        {
            return ContainsKey(index) && this[index].Contains(item);
        }

        public void AddTo(TIndex index, TItem item)
        {
            Contract.Requires<ArgumentException>(ContainsKey(index), "Key does not exist in the collection");
            collectionMap[index].Add(item);
        }

        public void RemoveFrom(TIndex index, TItem item)
        {
            Contract.Requires<ArgumentException>(ContainsKey(index), "index");
            collectionMap[index].Remove(item);
        }

        public void DefineNew(TIndex index)
        {
            Contract.Requires<ArgumentNullException>(index != null, "index");
            collectionMap.Add(index, new TCollection());
        }

        public void Remove(TIndex index)
        {
            collectionMap.Remove(index);
        }

        public IEnumerable<TItem> Select(TIndex index)
        {
            return !ContainsKey(index) ? Enumerable.Empty<TItem>() : collectionMap[index];
        }

        public TCollection this[TIndex index]
        {
            get { return collectionMap[index]; }
            set { collectionMap[index] = value; }
        }

        public IEnumerator<TCollection> GetEnumerator()
        {
            return collectionMap.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
