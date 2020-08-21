using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace Infrastructure.Web.Domain.Entity
{
    [Table("SysRoleMenu")]
    public partial class SysRoleMenuModel : EntityBase<int>
	{

		/// <summary>
		///     Role name
		/// </summary>
        public int? RolesID
		{
			get;
			set;
		}

        public int? MenuID
        {
            get;
            set;
        }

 
	}
}
