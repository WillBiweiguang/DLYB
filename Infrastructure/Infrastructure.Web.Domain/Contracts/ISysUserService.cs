using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Microsoft.AspNet.Identity;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Services;
using System.Security.Principal;
namespace Infrastructure.Web.Domain.Contracts
{
    public interface ISysUserService : IDependency,IBaseService<SysUser>
    {
         UserManager<SysUser, int> UserContext { get; set; }
         SysUser UserLoginAsync(string strUser, string strPassword);

         SysUser AutoLogin(IIdentity objWI);
         SysUser AutoLogin(string strUser);
    }

    public interface ISysRoleService : IDependency, IBaseService<SysRole>
    {
        RoleManager<SysRole, int> RoleContext { set; get; }
        
    }

    public interface ISysMenuService : IDependency, IBaseService<SysMenu>
    {
        List<SysMenu> GetMenusByUserID(SysUser objUser, UserStore manager);
    }


    public interface ISysRoleMenuService : IDependency, IBaseService<SysRoleMenuModel>
    {
         bool SetRoleMenu(int iRoleID, string strMenus);
         List<int> GetMenusByRoleID(int iRoleID);
    }

    public interface ISysUserRoleService : IDependency, IBaseService<SysUserRole>
    {
        bool CheckExistUserRoleUsed(int roleId);
    }
}
