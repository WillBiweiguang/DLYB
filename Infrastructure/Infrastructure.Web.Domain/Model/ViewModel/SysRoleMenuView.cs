using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
	//[Table("Push")]
    public partial class SysRoleMenuView : IViewModel
	{
	
		public Int32 Id { get;set; }

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

 
 
        public IViewModel ConvertAPIModel(object obj){
        var entity = (SysRoleMenuModel)obj;
	    Id =entity.Id;
        RolesID = entity.RolesID;
        MenuID = entity.MenuID;

 
        return this;
        }
	}
}
