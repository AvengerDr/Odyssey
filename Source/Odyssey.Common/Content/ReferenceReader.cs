using System.Runtime.Serialization;
using Odyssey.Engine;
using Odyssey.Utilities.Logging;

namespace Odyssey.Content
{
    public class ReferenceReader : IContentReader
    {
        public object ReadContent(IAssetProvider assetManager, ref ContentReaderParameters parameters)
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(EngineReference[]));
                var data = (EngineReference[])serializer.ReadObject(parameters.Stream);
                return new EngineReferenceCollection(data);
            }
            catch (SerializationException e)
            {
                LogEvent.Tool.Error(e.Message);
                throw;
            }
        }
    }
}
