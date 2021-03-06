using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Web.Domain.Entity
{
    [Table("t_sysUserLogin")]
    /// <summary>
    /// 外部登录用，比如QQ集成等
    /// </summary>
    public partial class SysUserLogin : EntityBase<int>
	{
        /// <summary>
        ///     The login provider for the login (i.e. facebook, google)
        /// </summary>
        public  string LoginProvider
        {
            get;
            set;
        }
        /// <summary>
        ///     Key representing the login for the provider
        /// </summary>
        public  string ProviderKey
        {
            get;
            set;
        }
        /// <summary>
        ///     User Id for the user who owns this login
        /// </summary>
        public  int UserId
        {
            get;
            set;
        }
	}
}
