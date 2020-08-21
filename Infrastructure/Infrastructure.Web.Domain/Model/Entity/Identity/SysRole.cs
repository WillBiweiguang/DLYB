using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysRole : EntityBase<int>, IRole<int>
	{
       		/// <summary>
		///     Navigation property for users in the role
		/// </summary>
        public virtual ICollection<SysUserRole> Users
        {
            get;
            private set;
        }
		/// <summary>
		///     Role id
		/// </summary>
		public override int Id
		{
			get;
			set;
		}
		/// <summary>
		///     Role name
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		/// <summary>
		///     Constructor
		/// </summary>
		public SysRole()
		{
			this.Users = new List<SysUserRole>();
		}

        public bool? IsDeleted { get; set; }
 
	}
}
