using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;


namespace Infrastructure.Web.Domain.Service.Common
{
    public class AuthenticationService : IAuthenticationService {

        public ISysUserService UserManager { get; private set; }
        public AuthenticationService(ISysUserService userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// 登录流程
        /// </summary>
        /// <param name="user"></param>
        /// <param name="createPersistentCookie"></param>
        public async  Task SignInAsync(IUser<int> user, bool createPersistentCookie)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.UserContext.CreateIdentityAsync((SysUser)user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = createPersistentCookie }, identity);
            HttpContext.Current.Session["UserInfo"] = user;

        }
        public async Task SignInNoDB(IUser<int> user, bool createPersistentCookie)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            UserManager.UserContext = new UserManager<SysUser, int>(new UserStoreNoDB());

           var identity = await UserManager.UserContext.CreateIdentityAsync((SysUser)user, DefaultAuthenticationTypes.ApplicationCookie);
            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, this.UserNameClaimType, this.RoleClaimType);
            //claimsIdentity.AddClaim(new Claim(this.UserIdClaimType, this.ConvertIdToString(user.Id), "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new Claim(this.UserNameClaimType, user.UserName, "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"));
			
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = createPersistentCookie }, identity);
            HttpContext.Current.Session["UserInfo"] = user;

        }

        /// <summary>
        /// 注销
        /// </summary>
       public  void SignOut() {
           AuthenticationManager.SignOut();
           HttpContext.Current.Session.RemoveAll();
          // return HttpContext.Current.Request.r("Index", "Home");
       
       }

        /// <summary>
        /// 设置用户
        /// </summary>
        /// <param name="user"></param>
       public void SetAuthenticatedUserForRequest(IUser<int> user) { }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return ((HttpContextBase)new HttpContextWrapper(HttpContext.Current)).GetOwinContext().Authentication;
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="Identity"></param>
        /// <returns></returns>
        public IUser<int> GetAuthenticatedUser(IIdentity Identity)
        {

            //var httpContext = _httpContextAccessor.Current();
            //if (httpContext.IsBackgroundContext() || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity)) {
            //    return null;
            //}

            var user = HttpContext.Current.Session["UserID"] as SysUser;
            if (user != null)
            {
                return user;
            }

            var formsIdentity = Identity;
            var userData = formsIdentity.GetUserId();

            if (Identity is WindowsIdentity)
            {
                return new SysUser() { Id = -1, UserName = formsIdentity .GetUserName()};
            }
            else
            {
                int userId;
                if (!int.TryParse(userData, out userId))
                {
                   // Logger.Error("User id not a parsable integer");
                    return null;
                }
                return new SysUserService().GetLoginUser(userId);
            }
        }



    }
}
