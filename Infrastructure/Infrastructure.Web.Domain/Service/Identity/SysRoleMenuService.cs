// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


using System.Data.Entity.Infrastructure;
using System.Globalization;

using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using Infrastructure.Web.Domain.Service.Common;
using System.Security.Principal;



namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class SysRoleMenuService : BaseService<SysRoleMenuModel>, ISysRoleMenuService
    {
        public SysRoleMenuService()
        {

        }

        public SysRoleMenuService(IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {

        }

        public bool SetRoleMenu(int iRoleID, string strMenus)
        {



            Repository.Delete(a => a.RolesID == iRoleID);

            if (string.IsNullOrEmpty(strMenus)) { return true; }

            var strs = strMenus.Split(',');
            List<SysRoleMenuModel> lst = new List<SysRoleMenuModel>();
            foreach (var s in strs)
            {
                SysRoleMenuModel d = new SysRoleMenuModel()
                {
                    MenuID = int.Parse(s),
                    RolesID = iRoleID
                };
                lst.Add(d);


            }
           Repository.Insert((IEnumerable<SysRoleMenuModel>)lst);

            return true;


        }



        public List<int> GetMenusByRoleID(int iRoleID)
        {
            return Repository.Entities.Where(a => a.RolesID == iRoleID).Select(a => a.MenuID.Value).ToList();
        }
    }


    public partial class SysMenuService : BaseService<SysMenu>, ISysMenuService
    {        
        //保存已登录过的父类菜单列表
        List<SysMenu> parentMenuList = new List<SysMenu>();
        //保存已登录过的子类菜单列表
        List<SysMenu> childMenuList = new List<SysMenu>();

        public List<SysMenu> GetMenusByUserID(SysUser objUser, UserStore manager)
        {

            var q = (from a in Repository.Entities.Where(aa => aa.IsDeleted == false)
                     join b in new SysRoleMenuService(Repository.UnitOfWork).Repository.Entities on a.Id equals b.MenuID
                     join c in new BaseService<SysRole>(Repository.UnitOfWork).Repository.Entities on b.RolesID equals c.Id
                     join d in new BaseService<SysUserRole>(Repository.UnitOfWork).Repository.Entities.Where(x => x.UserId == objUser.Id) on c.Id equals d.RoleId
                     select a).Distinct();

            // 重新设定数据集
            List<SysMenu> userMenu = q.ToList();
            // 取得所有可用Menu菜单（节点）信息
            List<SysMenu> allMenus = Repository.Entities.Where(x => x.IsDeleted == false).Distinct().ToList();

            parentMenuList = new List<SysMenu>();
            childMenuList = new List<SysMenu>();
            //记录关联的父类菜单
            foreach (SysMenu menu in userMenu)
            {
                SetParentSysMenuList(allMenus, menu);
            }

            //记录关联的父类菜单
            foreach (SysMenu menu in userMenu)
            {
                SetChildSysMenuList(allMenus, menu);
            }

            // 将父节点信息添加到列表中
            if (parentMenuList.Count != 0)
            {
                foreach (SysMenu menu in parentMenuList)
                {
                    // 防止与旧有数据冲突重复
                    if (!userMenu.Contains(menu))
                    {
                        userMenu.Add(menu);
                    }
                }
            }

            // 将子节点信息添加到列表中
            if (childMenuList.Count != 0)
            {
                foreach (SysMenu menu in childMenuList)
                {
                    // 防止与旧有数据冲突重复
                    if (!userMenu.Contains(menu))
                    {
                        userMenu.Add(menu);
                    }
                }
            }

            //return q.ToList();
            return userMenu;
        }

        /// <summary>
        /// 登录关联的父类菜单
        /// </summary>
        /// <param name="allMenu">Menu总集</param>
        /// <param name="activeMenu">当前授权菜单信息</param>
        private void SetParentSysMenuList(List<SysMenu> allMenu, SysMenu activeMenu)
        {
            if (activeMenu.ParentID != 1)
            {
                SysMenu parentMenu = allMenu.Where(x => x.Id == activeMenu.ParentID).Distinct().FirstOrDefault();
                //若是未登录过的父节点菜单，追加父节点信息
                if (parentMenu != null && !parentMenuList.Contains(parentMenu))
                {
                    parentMenuList.Add(parentMenu);
                    SetParentSysMenuList(allMenu, parentMenu);
                }
            }           
        }

        /// <summary>
        /// 登录关联的子类菜单
        /// </summary>
        /// <param name="allMenu">Menu总集</param>
        /// <param name="activeMenu">当前授权菜单信息</param>
        private void SetChildSysMenuList(List<SysMenu> allMenu, SysMenu activeMenu)
        {
            List<SysMenu> childMenu = allMenu.Where(x => x.ParentID == activeMenu.Id).Distinct().ToList();
            if (childMenu != null && childMenu.Count != 0)
            {
                foreach (SysMenu child in childMenu)
                {
                    // 防止与旧有数据冲突重复
                    if (!childMenuList.Contains(child))
                    {
                        childMenuList.Add(child);
                        SetChildSysMenuList(allMenu, child);
                    }
                }
            }
        } 
    }

    public partial class SysUserRoleService : BaseService<SysUserRole>, ISysUserRoleService
    {
        public SysUserRoleService()
        {

        }
        
        public bool CheckExistUserRoleUsed(int roleId)
        {
            bool result = true;

            int usedUserCount = (from a in Repository.Entities.Where(x => x.RoleId == roleId)
                     join b in new BaseService<SysUser>(Repository.UnitOfWork).Repository.Entities.Where(x => x.IsDeleted == false) on a.UserId equals b.Id
                     select a).Distinct().Count();

            if (usedUserCount == 0)
            {
                result = false;
            }

            return result;
        }
    }

}