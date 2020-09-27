﻿using DLYB.Web.Controllers;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DLYB.Web.Controllers
{
    public class PlatformManageController : BaseController<SysRole, SysRoleView>
    {
        ISysRoleService _roleService;
        ISysRoleMenuService ServiceRoleMenu;

        public PlatformManageController(
            ISysRoleService roleService,
            ISysRoleMenuService serviceRoleMenu) : base(roleService)
        {
            _roleService = roleService;
            ServiceRoleMenu = serviceRoleMenu;
        }

        // GET: PlateformManage
        public override ActionResult Index()
        {
            var user = Session["UserInfo"] as SysUser;
            //user 包含System Admin
            //if (user != null && user.Apps.Contains(-1))
            //{
            //    ViewBag.NeedLoadRoels = true;
            //    ViewBag.Roles = _roleService.Repository.Entities.Where(a => !a.IsDeleted.Value && a.Name != "Super Admin").ToList();
            //}
            ViewBag.NeedLoadRoels = true;
            ViewBag.Roles = _roleService.Repository.Entities.Where(a => !a.IsDeleted.Value && a.Name != "Super Admin").ToList();
            ViewBag.IsRole = false;
            ViewBag.IsUser = false;
            if (user != null && user.Menus != null)
            {
                if (user.Menus.Where(p => p.MenuName == "权限管理" & p.MenuGroup == "System Admin").Count() > 0)
                {
                    ViewBag.IsRole = true;
                }
                if (user.Menus.Where(p => p.MenuName == "用户管理" & p.MenuGroup == "System Admin").Count() > 0)
                {
                    ViewBag.IsUser = true;
                }
            }
            if (user != null && user.UserName == "administrator")
            {
                ViewBag.IsRole = true;
                ViewBag.IsUser = true;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult PasswordChange()
        {
            return View();
        }

        public ActionResult SetMenu(string Menus, int RoleID)
        {
            ServiceRoleMenu.SetRoleMenu(RoleID, Menus);
            Session.Clear();
            return SuccessNotification("OK");
        }
    }
}