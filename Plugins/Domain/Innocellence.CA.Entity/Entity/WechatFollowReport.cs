using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.Entity
{

    public partial class WechatFollowReport : EntityBase<int>
	{

        public override Int32 Id { get; set; }

        public DateTime? StatisticsDate { get; set; }

        public Int32? FollowCount { get; set; }

        public Int32? UnFollowCount { get; set; }

        public DateTime? CreatedDate { get; set; }
 
	}
}
