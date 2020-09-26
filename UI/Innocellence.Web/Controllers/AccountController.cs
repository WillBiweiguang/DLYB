using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

using Infrastructure.Core.Data;
using Infrastructure.Web.UI;
//using Innocellence.Web.ModelsView;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service;
using DLYB.CA.Contracts.Contracts;
using DLYB.Web.Service;

namespace Innocellence.Web.Controllers
{
    //[Authorize]
    public class AccountController : ParentController<SysUser, SysUserView>
    {
        private readonly ILoginService _loginService;
        public AccountController(ISysUserService userManager, IAuthenticationService authService, IOauthClientDataService clientDataService
            ,ILoginService loginService)
            : base(userManager)
        {
            UserManager = userManager;
            //   UserAdmin = new UserManager<SysUser, int>(new UserStore());
            _authService = authService;
            _clientDataService = clientDataService;
            _loginService = loginService;
        }

        private readonly IAuthenticationService _authService;


        private readonly IOauthClientDataService _clientDataService;
        public ISysUserService UserManager { get; private set; }

        //  public UserManager<SysUser, int> UserAdmin { get;private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //windows登录
            //if ((Request.IsAuthenticated && (User.Identity is WindowsIdentity)))
            //{
            //    return Redirect("~/Course/index");
            //}
            //验证是否已登录。
            System.Web.Configuration.AuthenticationSection section =
  (System.Web.Configuration.AuthenticationSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/authentication");


            ViewBag.Mode = section.Mode;
            // ViewBag.UserName = User.Identity.Name;
            ViewBag.ReturnUrl = returnUrl;
            //string uuid = Guid.NewGuid().ToString();
            //ViewBag.Uuid = uuid;
            //ViewBag.Captcha = _loginService.GetCaptcha(uuid);
            return View();
        }
        //TODO 获取验证码


        //TODO 暂不做验证码，稍后加入
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {            
            if (ModelState.IsValid)
            {
                //暂时去掉用户登录
                var user = new SysUser { Id = 1, UserName = model.UserName };
                model.RememberMe = true;
                await _authService.SignInNoDB(user, true);
                return Json(doJson(null, returnUrl), JsonRequestBehavior.AllowGet);
                // BaseService<SysUserClaim> a = new BaseService<SysUserClaim>();

                // await ((IDbSet<SysUserClaim>)a.Entities).Where(uc => uc.UserId==9).LoadAsync();
                //a.Entities.ToList();
                //var user = UserManager.UserLoginAsync(model.UserName, model.Password);
                var result = _loginService.Login(model.UserName, model.Password);
                if (result)
                {
                    // await SignInAsync(user, model.RememberMe);
                    // return RedirectToLocal(returnUrl);
                     user = new SysUser { Id = 1, UserName = model.UserName };
                    model.RememberMe = true;
                    await _authService.SignInNoDB(user, model.RememberMe);

                    //登录日志
                    //BaseService<LogsModel> objServLogs = new BaseService<LogsModel>();
                    //objServLogs.Repository.Insert(new LogsModel() { LogCate = "AdminLogin", LogContent = "登录成功", CreatedUserName = model.UserName });
                    
                    return Json(doJson(null, returnUrl), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
                    // ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new SysUser() { UserName = model.UserName };
                var result = await UserManager.UserContext.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.UserContext.RemoveLoginAsync(int.Parse(User.Identity.GetUserId()), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.UserContext.ChangePasswordAsync(int.Parse(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.UserContext.AddPasswordAsync(int.Parse(User.Identity.GetUserId()), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }



        public ActionResult BackUrl()
        {
            // 请求重定向到外部登录提供程序
            return Json("aaa", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/ExternalLogin

        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.UserContext.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.UserContext.AddLoginAsync(int.Parse(User.Identity.GetUserId()), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new SysUser() { UserName = model.UserName };
                var result = await UserManager.UserContext.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.UserContext.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        // [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult LogOff(string returnUrl, string clientid)
        {
            //AuthenticationManager.SignOut();
            //Session.RemoveAll();
            var isReturnExternal = false;

            if (!string.IsNullOrEmpty(returnUrl) && !string.IsNullOrEmpty(clientid))
            {
                var clientUrl = _clientDataService.Repository.Entities.Where(x => x.ClientId == clientid).Select(x => x.ClientCallBackUrl).FirstOrDefault();

                if (string.IsNullOrEmpty(clientUrl))
                {
                    throw new ArgumentException(@"clientid 不正确!", clientid);
                }

                var targetHost = new Uri(returnUrl).Host;
                var configedHost = new Uri(clientUrl).Host;

                if (targetHost != configedHost)
                {
                    throw new ArgumentException(@"返回地址不合法!");
                }

                isReturnExternal = true;
            }
            else if (!(string.IsNullOrEmpty(returnUrl) && string.IsNullOrEmpty(clientid)))
            {
                throw new ArgumentNullException(string.IsNullOrEmpty(returnUrl) ? "returnUrl" : "clientid");
            }

            _authService.SignOut();

            if (isReturnExternal)
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.UserContext.GetLogins(int.Parse(User.Identity.GetUserId()));
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.UserContext.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region 帮助程序
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(SysUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.UserContext.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            Session["UserInfo"] = user;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.UserContext.FindById(int.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri, IsPersistent = false };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}