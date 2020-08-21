using System.Web;

namespace Infrastructure.Utility.Localization.Services
{
    public class CultureSelectorResult {
        public int Priority { get; set; }
        public string CultureName { get; set; }
    }

    public interface ICultureSelector : IDependency {
        CultureSelectorResult GetCulture(HttpContextBase context);
    }
}
