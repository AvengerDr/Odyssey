using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Odyssey.UserInterface.Style;

namespace Odyssey.Content
{
    public class ThemeReader : IContentReader
    {
        public object ReadContent(IAssetProvider assetManager, ref ContentReaderParameters parameters)
        {
            var xmlReaderSettings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };
            var content = parameters.Services.GetService<IAssetProvider>();
            Theme theme = content.Contains(parameters.AssetName) ? content.Load<Theme>(parameters.AssetName) : new Theme();
            theme.DeserializeXml(theme, XmlReader.Create(parameters.Stream, xmlReaderSettings));

            return theme;
        }
    }
}
