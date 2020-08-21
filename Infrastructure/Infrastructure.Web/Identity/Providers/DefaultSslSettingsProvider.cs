using System.Web.Security;

namespace Infrastructure.Web.Identity.Providers {
    public class DefaultSslSettingsProvider : ISslSettingsProvider {
        public bool RequireSSL { get; set; }

        public DefaultSslSettingsProvider() {
            RequireSSL = FormsAuthentication.RequireSSL;
        }

        public bool GetRequiresSSL() {
            return RequireSSL;
        }
    }
}
