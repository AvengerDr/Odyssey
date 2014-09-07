using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Graphics.Models;

namespace Odyssey.Talos.Messages
{
    public class ContentMessage<TContent> : EntityMessage
    {
        public string AssetName { get; private set; }
        public TContent Content { get; private set; }

        public ContentMessage(Entity source, string assetName, TContent content, bool isSynchronous = false)
            : base(source, isSynchronous)
        {
            AssetName = assetName;
            Content = content;
        }
    }
}
