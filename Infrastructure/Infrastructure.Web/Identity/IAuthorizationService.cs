
using Infrastructure.Web.Identity.Permissions;
using Infrastructure.Core;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Security.Principal;

namespace Infrastructure.Web.Identity {
    /// <summary>
    /// Entry-point for configured authorization scheme. Role-based system
    /// provided by default. 
    /// </summary>
    public interface IAuthorizationService : IDependency {
        void CheckAccess(Permission permission, IUser<int> user);
        bool TryCheckAccess(Permission permission, IUser<int> user);

        bool TryCheckAccess(Permission permission, AuthorizationContext filterContext, IUser<int> user);

        bool TryCheckAccess(AuthorizationContext filterContext, IUser<int> user);

        IUser<int> GetUser(IIdentity objWI);


        
    }
}