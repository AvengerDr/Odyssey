using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Talos.Systems;
using Odyssey.Utilities.Collections;
using Odyssey.Utilities.Logging;

namespace Odyssey.Talos.Maps
{
    public class MessageMap
    {
        private readonly CollectionMap<Type, List<SystemBase>, SystemBase> collectionMap;

        public MessageMap()
        {
            collectionMap = new CollectionMap<Type, List<SystemBase>, SystemBase>();
        }

        private void Associate(Type index)
        {
            if (!collectionMap.ContainsKey(index))
            {
                collectionMap[index] = new List<SystemBase>();
            }
            else LogEvent.Engine.Warning("Map is already associated to " + index.Name);
        }

        public void Associate<TDerivedIndex>()
        {
            Associate(typeof (TDerivedIndex));
        }

        public void RemoveAssociation<TDerivedIndex>()
        {
            RemoveAssociation(typeof (TDerivedIndex));
        }

        private void RemoveAssociation(Type messageType)
        {
            collectionMap.Remove(messageType);
        }

        private bool IsAssociated(Type messageType)
        {
            return collectionMap.ContainsKey(messageType);
        }

        public bool IsAssociated<TDerivedIndex>()
        {
            return IsAssociated(typeof (TDerivedIndex));
        }

        private bool IsMapEmpty(Type messageType)
        {
            return collectionMap[messageType].Count == 0;
        }

        [Pure]
        public bool IsRegistered<TDerivedIndex>(SystemBase item)
        {
            Type indexType = typeof (TDerivedIndex);

            if (!IsAssociated(indexType))
                return false;
            return collectionMap[indexType].Contains(item);
        }


        public void Add<TDerivedIndex>(SystemBase item)
        {
            Contract.Requires<ArgumentException>(!IsRegistered<TDerivedIndex>(item));
            Type indexType = typeof (TDerivedIndex);

            if (!IsAssociated(indexType))
                Associate(indexType);

            collectionMap[indexType].Add(item);
        }

        public void Remove<TDerivedIndex>(SystemBase item)
        {
            Contract.Requires<ArgumentException>(IsRegistered<TDerivedIndex>(item));
            Type messageType = typeof (TDerivedIndex);

            collectionMap[messageType].Remove(item);

            if (IsMapEmpty(messageType))
                RemoveAssociation(messageType);
        }

        public IEnumerable<SystemBase> Select<TDerivedIndex>()
        {
            Type messageType = typeof (TDerivedIndex);
            if (!collectionMap.ContainsKey(messageType))
                return Enumerable.Empty<SystemBase>();

            var subscribedSystems = collectionMap[messageType];

            if (subscribedSystems.Any())
                return subscribedSystems;
            else
                return Enumerable.Empty<SystemBase>();
        }
    }

}
