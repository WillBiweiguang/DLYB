using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Identity;
using Microsoft.AspNet.Identity;
using Infrastructure.Web.UI;
using System.Net;


namespace Infrastructure.Web.MVC.Attribute
{
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CustomAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool _dontValidate;
        private readonly IAuthorizer _authorizer;
        private readonly string _strLoginUrl;

        public CustomAuthorizeAttribute()
            : this(false)
        {
        }

        /// <summary>
        /// dontValidate:true 不验证权限，只判断是否登录
        /// </summary>
        /// <param name="dontValidate"></param>
        public CustomAuthorizeAttribute(bool dontValidate)
        {
            this._dontValidate = dontValidate;
            _authorizer = EngineContext.Current.Resolve<IAuthorizer>();
        }

        public CustomAuthorizeAttribute(string strLoginUrl)
        {
            _strLoginUrl = strLoginUrl;
            //this._dontValidate = dontValidate;
            _authorizer = EngineContext.Current.Resolve<IAuthorizer>();
        }

        protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (string.IsNullOrEmpty(_strLoginUrl)) { 
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectResult(_strLoginUrl);
            }
            
        }

        private IEnumerable<AdminAuthorizeAttribute> GetAdminAuthorizeAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(AdminAuthorizeAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AdminAuthorizeAttribute), true))
                .OfType<AdminAuthorizeAttribute>();
        }

        private bool IsAdminPageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetAdminAuthorizeAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
                return true;
            return false;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var objLoginInfo = filterContext.HttpContext.Session["UserInfo"] as IUser<int>;

            //var accessFrontEnd = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
            //if (accessFrontEnd) { return; }

            bool flag = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) 
                       || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (flag)
            {
                return;
            }

            //windows登录,自动获取用户信息
            if (objLoginInfo == null && (filterContext.HttpContext.Request.IsAuthenticated && (filterContext.HttpContext.User.Identity != null)))
            {
                var windowsIdentity = filterContext.HttpContext.User.Identity;
                var objUser = _authorizer.GetUser(windowsIdentity);
                if (objUser != null)
                {
                    objLoginInfo = objUser;
                    filterContext.HttpContext.Session["UserInfo"] = objLoginInfo;
                }
                else
                {
                   // filterContext.Result = new HttpUnauthorizedResult();
                    HandleUnauthorizedRequest(filterContext);
                    return;
                }
            }
            else if (!filterContext.HttpContext.Request.IsAuthenticated || filterContext.HttpContext.User.Identity == null)
            {
                UnauthRedirect(filterContext);
                return;
            }


            flag = filterContext.ActionDescriptor.IsDefined(typeof(AllowLoginUserAttribute), true)
                       || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowLoginUserAttribute), true);
            if (flag)
            {
                return;
            }
            

            //var ca = filterContext.ActionDescriptor.GetCustomAttributes(typeof(CustomAuthorizeAttribute), true).Any(a => ((CustomAuthorizeAttribute)a)._dontValidate)
             //      ||filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(CustomAuthorizeAttribute), true).Any(a => ((CustomAuthorizeAttribute)a)._dontValidate);
            if (_dontValidate)
                return;

            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");

            

            //if (!AdminFilter.IsApplied(filterContext.RequestContext) && !accessFrontEnd && !_authorizer.Authorize(StandardPermissions.AccessFrontEnd)) {
            if ( !_authorizer.Authorize(filterContext, objLoginInfo))
            {
                UnauthRedirect(filterContext);
            }
        }


        private void UnauthRedirect(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                AjaxResult<int> result = new AjaxResult<int>();
                result.Message = new JsonMessage((int)HttpStatusCode.Unauthorized, "Please login");
                filterContext.Result = new JsonResult
                {
                    Data = result,
                    //ContentType = contentType,
                    //ContentEncoding = contentEncoding,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        public virtual bool HasAdminAccess(AuthorizationContext filterContext)
        {
            // var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            // bool result = permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel);
            // return result;
            return true;
        }
    }

   public sealed class AllowLoginUserAttribute : System.Attribute
   {
      
   }

}
