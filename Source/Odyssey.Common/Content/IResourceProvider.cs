using System.Collections.Generic;

namespace Odyssey.Content
{
    public interface IResourceProvider : IResource
    {
        bool ContainsResource(string resourceName);
        TResource GetResource<TResource>(string resourceName)
            where TResource : class, IResource;

        IEnumerable<IResource> Resources { get; }
    }
}