using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysUserRole : EntityBase<int>
	{

        public override int Id { get; set; }

        /// <summary>
        ///     UserId for the user that is in the role
        /// </summary>
        public  int UserId
        {
            get;
            set;
        }
        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public  int RoleId
        {
            get;
            set;
        }
 
	}
}
