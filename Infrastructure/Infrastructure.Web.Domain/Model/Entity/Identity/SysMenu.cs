using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysMenu : EntityBase<int>
	{

        /// <summary>
        ///     UserId for the user that is in the role
        /// </summary>
        public override int Id
        {
            get;
            set;
        }
        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public  string  MenuName
        {
            get;
            set;
        }
        public int? ParentID { get; set; }
        public string MenuTitle { get; set; }

        public int? FormID { get; set; }

        public string MenuImg { get; set; }
        public string MenuGroup { get; set; }
        

        public int? MenuType { get; set; }

        public string NavigateUrl { get; set; }

        public int? SortCode { get; set; }

        public bool? IsDisplay { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserID { get; set; }
        public bool? IsDeleted { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? AppId { get; set; }
	}
}
