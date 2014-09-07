using Odyssey.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Odyssey.Utilities.Collections
{
    public class QueueMap<TItem>
    {
        readonly Dictionary<Type, Queue<TItem>> queueMap;

        public QueueMap()
        {
            queueMap = new Dictionary<Type, Queue<TItem>>();
        }

        void Associate(Type messageType)
        {
            if (!queueMap.ContainsKey(messageType))
            {
                queueMap[messageType] = new Queue<TItem>();
            }
            else LogEvent.Engine.Warning("Map is already associated to " + messageType.Name);
        }

        public void Associate<TDerivedItem>()
            where TDerivedItem : TItem
        {
            Associate(typeof(TDerivedItem));
        }

        public void RemoveAssociation<TDerivedItem>()
            where TDerivedItem : TItem
        {
            RemoveAssociation(typeof(TDerivedItem));
        }

        void RemoveAssociation(Type messageType)
        {
            queueMap.Remove(messageType);
        }

        bool IsAssociated(Type messageType)
        {
            return queueMap.ContainsKey(messageType);
        }

        [Pure]
        public bool IsAssociated<TDerivedItem>()
            where TDerivedItem : TItem
        {
            return IsAssociated(typeof(TDerivedItem));
        }

        bool IsMapEmpty(Type messageType)
        {
            return !queueMap.ContainsKey(messageType) || queueMap[messageType].Count == 0;
        }

        public bool HasItems<TDerivedItem>()
            where TDerivedItem : TItem
        {
            return !IsMapEmpty(typeof(TDerivedItem));
        }


        public virtual void Enqueue<TDerivedItem>(TItem item)
            where TDerivedItem : TItem
        {
            Contract.Requires<ArgumentNullException>(item!=null, "item");
            Type indexType = item.GetType();

            if (!IsAssociated(indexType))
                Associate(indexType);

            queueMap[indexType].Enqueue(item);
        }

        public virtual TDerivedItem Dequeue<TDerivedItem>()
             where TDerivedItem : TItem
        {
            Type messageType = typeof (TDerivedItem);
            TDerivedItem item = (TDerivedItem)queueMap[messageType].Dequeue();

            return item;
        }

    }
}
