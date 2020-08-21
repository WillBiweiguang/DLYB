using Infrastructure.Core;
namespace Infrastructure.Web.Localization.Services
{
    public interface ILocalizedStringManager  {
        string GetLocalizedString(string scope, string text, string cultureName);
    }
}
