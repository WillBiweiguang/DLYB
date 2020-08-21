using Infrastructure.Core;
using System;

namespace Infrastructure.Web.Identity {
    public interface ISslSettingsProvider : IDependency {

        /// <summary>
        /// Gets whether authentication cookies should only be transmitted over SSL or not.
        /// </summary>
        bool GetRequiresSSL();
    }
}
