using Infrastructure.Core;
using System;

namespace DLYB.CA.Contracts.Entity
{
    public class MenuReportEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string UserId { get; set; }

        public string MenuKey { get; set; }

        public DateTime AccessDate { get; set; }

        public int AppId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MenuName { get; set; }

        public string AppName { get; set; }

        /// <summary>
        /// 访问人数
        /// </summary>
        public int VisitorCount { get; set; }

        /// <summary>
        /// 访问次数
        /// </summary>
        public int VisitTimes { get; set; }
    }
}
