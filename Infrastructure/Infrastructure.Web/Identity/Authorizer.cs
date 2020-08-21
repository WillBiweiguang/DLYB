
using Infrastructure.Web.Identity.Permissions;

using Infrastructure.Core;
using Infrastructure.Web.Localization;
using Infrastructure.Core.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Security.Principal;

namespace Infrastructure.Web.Identity {
    /// <summary>
    /// Authorization services for the current user
    /// </summary>
    public interface IAuthorizer : IDependency {
        /// <summary>
        /// Authorize the current user against a permission
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        bool Authorize(Permission permission);

        /// <summary>
        /// Authorize the current user against a permission; if authorization fails, the specified
        /// message will be displayed
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        /// <param name="message">A localized message to display if authorization fails</param>
        bool Authorize(Permission permission, LocalizedString message);


        bool Authorize(AuthorizationContext filterContext, IUser<int> User);

        ///// <summary>
        ///// Authorize the current user against a permission for a specified content item;
        ///// </summary>
        ///// <param name="permission">A permission to authorize against</param>
        ///// <param name="content">A content item the permission will be checked for</param>
        //bool Authorize(Permission permission, IContent content);

        ///// <summary>
        ///// Authorize the current user against a permission for a specified content item;
        ///// if authorization fails, the specified message will be displayed
        ///// </summary>
        ///// <param name="permission">A permission to authorize against</param>
        ///// <param name="content">A content item the permission will be checked for</param>
        ///// <param name="message">A localized message to display if authorization fails</param>
        //bool Authorize(Permission permission, IContent content, LocalizedString message);

         IUser<int> GetUser(IIdentity objWI);

    }

    public class Authorizer : IAuthorizer {
        private readonly IAuthorizationService _authorizationService;

        public Authorizer(
            IAuthorizationService authorizationService
           ) {
            _authorizationService = authorizationService;
        
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool Authorize(Permission permission) {
            return Authorize(permission,  null);
        }

        //public bool Authorize(Permission permission, LocalizedString message) {
        //    return Authorize(permission,  message);
        //}

        //public bool Authorize(Permission permission) {
        //    return Authorize(permission, content, null);
        //}

        public bool Authorize(AuthorizationContext filterContext,IUser<int> User)
        {

           // var User = EngineContext.Current.Resolve<WorkContextBase>().CurrentUser as IUser<int>;
            if (_authorizationService.TryCheckAccess(filterContext, User))
                return true;

            return false;
        }

        public bool Authorize(Permission permission, LocalizedString message) {

            var User = EngineContext.Current.Resolve<WorkContextBase>().CurrentUser as IUser<int>;
            if (_authorizationService.TryCheckAccess(permission, User))
                return true;

            if (message != null) {
                if (User == null)
                {
                   // _notifier.Error(T("{0}. Anonymous users do not have {1} permission.",
                                    //  message, permission.Name));
                }
                else {
                   // _notifier.Error(T("{0}. Current user, {2}, does not have {1} permission.",
                                     // message, permission.Name, User.UserName));
                }
            }

            return false;
        }

        public IUser<int> GetUser(IIdentity objWI)
        {
            return _authorizationService.GetUser(objWI);
        }

    }
}
