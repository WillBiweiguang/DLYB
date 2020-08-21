using System;
using System.Collections.Generic;
using Infrastructure.Core;
namespace DLYB.CA.Entity
{

    public class ArticlePublishInfo : EntityBase<int>
    {
        public override Int32 Id { get; set; }

        public int ArticleID { get; set; }
        public String Title { get; set; }
		public DateTime? CreatedDate { get;set; }
        public String CreatedUserID { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public String invaildDepartment { get; set; }
        public String invaildTag { get; set; }
        public String invaildUser { get; set; }

    }
}