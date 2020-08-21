using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysUserClaim : EntityBase<int>
	{

        /// <summary>
        ///     Primary key
        /// </summary>
        public override int Id
        {
            get;
            set;
        }
        /// <summary>
        ///     User Id for the user who owns this login
        /// </summary>
        public  int? UserId
        {
            get;
            set;
        }
        /// <summary>
        ///     Claim type
        /// </summary>
        public  string ClaimType
        {
            get;
            set;
        }
        /// <summary>
        ///     Claim value
        /// </summary>
        public  string ClaimValue
        {
            get;
            set;
        }
 
	}
}
