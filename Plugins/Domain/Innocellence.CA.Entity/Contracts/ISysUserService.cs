using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.CA.Entity;
using Microsoft.AspNet.Identity;
namespace Innocellence.CA.Contracts
{
    public interface ISysUserService : IDependency,IBaseService<SysUser>
    {
         UserManager<SysUser, int> UserContext { get; set; }
         SysUser UserLoginAsync(string strUser, string strPassword);
    }

    public interface ISysRoleService : IDependency, IBaseService<SysRole>
    {
        RoleManager<SysRole, int> RoleContext { set; get; }
        
    }
}
