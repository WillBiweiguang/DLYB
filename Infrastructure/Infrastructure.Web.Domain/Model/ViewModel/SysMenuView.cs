using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("ValidUser")]
    public partial class SysMenuView :IViewModel
	{

	
		/// <summary>
		///     User ID (Primary Key)
		/// </summary>
		public  int Id
		{
			get;
			set;
		}
        public string MenuName
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
        
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (SysMenu)obj;
            Id = entity.Id;
            MenuName = entity.MenuName;
            ParentID = entity.ParentID;
            MenuTitle = entity.MenuTitle;
            MenuImg = entity.MenuImg;
            MenuType = entity.MenuType;
            MenuGroup = entity.MenuGroup;
            NavigateUrl = entity.NavigateUrl;
            SortCode = entity.SortCode;
            IsDisplay = entity.IsDisplay;
            CreatedDate = entity.CreatedDate;
           // MenuTitle = entity.MenuTitle;
            AppId = entity.AppId;

            return this;
        }
 
	}
}
