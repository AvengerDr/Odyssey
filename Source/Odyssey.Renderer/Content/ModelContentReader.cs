using System.IO;
using Odyssey.Engine;
using Odyssey.Graphics.Models;

namespace Odyssey.Content
{
    /// <summary>
    /// Internal class to load a SpriteFont.
    /// </summary>
    internal class ModelContentReader : ResourceReaderBase<Model>
    {
        protected override Model ReadContent(IAssetProvider readerManager, DirectXDevice device, ref ContentReaderParameters parameters)
        {
            var assetPath = Path.GetDirectoryName(parameters.AssetName);

            // Loads the model.
            var model = Model.Load(device, parameters.Stream, parameters.AssetName);

            if (model == null)
            {
                return null;
            }

            // If the model has no name, use filename
            if (model.Name == null)
            {
                model.Name = Path.GetFileName(parameters.AssetName);
            }

            return model;
        }
    }
}
