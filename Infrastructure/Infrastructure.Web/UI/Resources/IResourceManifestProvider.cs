using Infrastructure.Core;
namespace Infrastructure.Web.UI.Resources
{
    public interface IResourceManifestProvider : ISingletonDependency {
        void BuildManifests(ResourceManifestBuilder builder);
    }
}
