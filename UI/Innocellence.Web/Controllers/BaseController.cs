
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web;
using Infrastructure.Core;
using Infrastructure.Core.Data;

using DLYB.Web.Entity;

using System.Linq.Expressions;
using System.Net;
using System.Security.Principal;

using DLYB.Web.Service;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Services;
using Infrastructure.Web.MVC.Attribute;
using Infrastructure.Web.Identity;
using Infrastructure.Core.Infrastructure;
//using DLYB.Authentication.Attribute;
//using DLYB.Web.Services;


namespace DLYB.Web.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]

    //[FilterError]
#if DEBUG
    [CustomAuthorize]
#else
    //[SSOOWinAuthorize("SAML2")]
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class BaseController<T, T1> : ParentController<T,T1>
        where T : EntityBase<int>,new()
        where T1 : IViewModel, new()
    {
        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _objService;
        private readonly IAuthorizationService _authorizationService;

        //全局用户对象，当前的登录用户
        public SysUser objLoginInfo;

        public int AppId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public BaseController(IBaseService<T> newsService)

            : base(newsService)
        {
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.DateFormatHandling
            //= Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

            _objService = newsService;
            _authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            //if (objLoginInfo != null)
            //{
            //    _newsService.LoginUsrID = objLoginInfo.UserID;
            //}


        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            objLoginInfo = Session["UserInfo"] as SysUser;
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnAuthorization(filterContext);
                return;
            }
            if (objLoginInfo != null && !_authorizationService.TryCheckAccess(filterContext, objLoginInfo))
            {
                //filterContext.Result = Redirect("~/Error/AuthError");
            }
            base.OnAuthorization(filterContext);
        }
        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            ////得到用户登录的信息
            objLoginInfo = Session["UserInfo"] as SysUser;

            if (objLoginInfo == null)
            {
                filterContext.Result = Redirect("~/Account/Login");
            }
            ////windows登录,自动获取用户信息
            //if (objLoginInfo == null && (Request.IsAuthenticated && (User.Identity !=null)))
            //{
            //    var windowsIdentity = User.Identity;

            //    SysUserService objServ = new SysUserService();
            //   var objUser= objServ.AutoLogin(windowsIdentity);
            //   if (objUser != null)
            //   {
            //       objLoginInfo = objUser;
            //       Session["UserInfo"] = objLoginInfo;
            //   }

            //}

            if (objLoginInfo == null)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    AjaxResult<int> result = new AjaxResult<int>();
                    result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, "Please login");
                    filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    //Redirect()
                    filterContext.Result = Redirect("~/Account/Login");
                }
            }
            // return;


            if (objLoginInfo != null)
            {
                _BaseService.Repository.LoginUserID = objLoginInfo.Id;
                _BaseService.Repository.LoginUserName = objLoginInfo.UserName;
                //SetLanguage("EN");
            }
            //进行权限验证
            //if (!objLoginInfo.Menus.Any(x => x.NavigateUrl.Contains(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName)))
            //{
            //filterContext.Result = Redirect("~/Error/AuthError");
            //}

            // System.Threading.Thread.Sleep(5000);

            //Logger log = Logger.GetLogger(filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName, CurrentUserInfo.USERREALNAME);
            //log.Debug(WEBUI.Common.LogHelper.GetActionInfo(filterContext));

            //lstMenus=Session["UserMenus"] as List<BASE_SYSMENU>;

            //主菜单控制
            ViewBag.IsManager = objLoginInfo?.Roles.Any(x => x.RoleId == 1); //主任
            ViewBag.IsAdmin = objLoginInfo?.Roles.Any(x => x.RoleId == 3 || x.RoleId == 4); //管理员
            ViewBag.UserTrueName = objLoginInfo?.UserTrueName;

            //用户信息
            ViewBag.CurUserId = objLoginInfo?.Id;
            base.OnActionExecuting(filterContext);
        }

        

    }
}
