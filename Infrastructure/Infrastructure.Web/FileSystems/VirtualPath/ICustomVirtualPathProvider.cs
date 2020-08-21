using System.Web.Hosting;

namespace Infrastructure.Web.FileSystems.VirtualPath
{
    public interface ICustomVirtualPathProvider {
        VirtualPathProvider Instance { get; }
    }
}