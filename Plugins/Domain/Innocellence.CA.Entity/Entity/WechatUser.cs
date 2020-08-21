using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.Entity
{
	//[Table("Logs")]
    public partial class WechatUser : EntityBase<int>
	{

        public override Int32 Id { get; set; }

        public String LanguageCode { get; set; }
        public String LillyID { get; set; }

        public String WechatID { get; set; }

		public  DateTime? CreatedDate { get;set; }

 
	}
}
