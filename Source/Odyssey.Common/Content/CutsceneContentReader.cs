using System.Xml;
using System.Xml.Serialization;
using Odyssey.Animations;
using Odyssey.Engine;

namespace Odyssey.Content
{
    internal class CutsceneContentReader : IContentReader
    {
        public object ReadContent(IAssetProvider assetManager, ref ContentReaderParameters parameters)
        {
            var xmlReaderSettings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            var xmlReader = XmlReader.Create(parameters.Stream, xmlReaderSettings);
            var cutscene = new Cutscene(parameters.Services);
            var scene = parameters.Services.GetService<IResourceProvider>();
            cutscene.DeserializeXml(scene, xmlReader);
            return cutscene;
        }
    }
}
