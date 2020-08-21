using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Web.Domain.Entity
{
	[Table("Logs")]
    public partial class LogsModel : EntityBase<int>
	{

        public  override Int32 Id { get; set; }
 
		public  string LogCate { get;set; }
		public  String LogContent { get;set; }
		public  Int32? LogFrom { get;set; }
		public  String LogSource { get;set; }
		public  DateTime? CreatedDate { get;set; }
		public  String CreatedUserID { get;set; }
		public  String CreatedUserName { get;set; }
 
	}
}
