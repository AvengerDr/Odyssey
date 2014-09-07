using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Odyssey.Utilities.Collections
{
    public class DictionaryMap<TIndex, TKey, TValue>
        where TKey : IComparable<TKey>
    {
        readonly Dictionary<TIndex, Dictionary<TKey, TValue>> collectionMap;

        public DictionaryMap()
        {
            collectionMap = new Dictionary<TIndex, Dictionary<TKey, TValue>>();
        }

        public bool ContainsKey(TIndex index)
        {
            Contract.Requires<ArgumentNullException>(index != null, "index");
            return collectionMap.ContainsKey(index);
        }

        public void AddTo(TIndex index, TKey key, TValue item)
        {
            Contract.Requires<ArgumentException>(ContainsKey(index), "Key does not exist in the collection");
            collectionMap[index].Add(key, item);
        }

        public void RemoveFrom(TIndex index, TKey key)
        {
            Contract.Requires<ArgumentException>(ContainsKey(index), "index");
            collectionMap[index].Remove(key);
        }

        public void DefineNew(TIndex index)
        {
            Contract.Requires<ArgumentNullException>(index != null, "index");
            collectionMap.Add(index, new Dictionary<TKey, TValue>());
        }

        public void Remove(TIndex index)
        {
            collectionMap.Remove(index);
        }

        public IEnumerable<TValue> Select(TIndex index)
        {
            return !ContainsKey(index) ? Enumerable.Empty<TValue>() : collectionMap[index].Values;
        }

        public Dictionary<TKey, TValue> this[TIndex index]
        {
            get { return collectionMap[index]; }
            set { collectionMap[index] = value; }
        }
    }
}
