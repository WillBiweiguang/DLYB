using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.CA.Entity;

namespace Innocellence.CA.ModelsView
{
	//[Table("Push")]
    public partial class SysRoleView : IViewModel
	{
	
		public Int32 Id { get;set; }

        public String Name { get; set; }

 
 
        public IViewModel ConvertAPIModel(object obj){
        var entity = (SysRole)obj;
	    Id =entity.Id;
        Name = entity.Name;

 
        return this;
        }
	}
}
