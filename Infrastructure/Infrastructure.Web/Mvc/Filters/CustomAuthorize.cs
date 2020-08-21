// -----------------------------------------------------------------------
//  <copyright file="NonRequireHttpsAttribute.cs" company="OSharp开源团队">
//      Copyright (c) 2014 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2014-11-13 21:39</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OSharp.Web.Mvc.Filters
{
    /// <summary>
    /// 将套字连接HTTPS转换为普通HTTP连接
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public new string[] Roles { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            if (Roles == null)
            {
                return true;
            }
            if (Roles.Length == 0)
            {
                return true;
            }
            if (Roles.Any(httpContext.User.IsInRole))
            {
                return true;
            }
            return false;
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string roles = "";// GetRoles.GetActionRoles(actionName, controllerName);
            if (!string.IsNullOrWhiteSpace(roles))
            {
                this.Roles = roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            base.OnAuthorization(filterContext);
        }
    }
}