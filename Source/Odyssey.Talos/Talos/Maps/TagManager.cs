using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Odyssey.Utilities.Collections;

namespace Odyssey.Talos.Maps
{
    public sealed class TagManager : IEnumerable<string>
    {
        private readonly CollectionMap<long, SortedSet<string>, string> tags;

        public TagManager()
        {
            tags = new CollectionMap<long, SortedSet<string>, string>();
        }

        public void AddTagToEntity(long entityId, string tag)
        {
            if (!tags.ContainsKey(entityId))
                tags.DefineNew(entityId);
            tags.AddTo(entityId, tag);
        }

        public void RemoveTagFromEntity(long entityId, string tag)
        {
            tags.RemoveFrom(entityId, tag);
        }

        public bool ContainsEntity(long entityId)
        {
            return tags.ContainsKey(entityId);
        }

        public bool ContainsTag(long entityId, string tag)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(tag), "tag");
            return tags.ContainsItem(entityId, tag);
        }

        public IEnumerator<string> GetEnumerator()
        {
            var values = from set in tags
                from s in set
                select s;
            return values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> this[long entityId]
        {
            get { return tags[entityId]; }
        }
    }
}
