using System;
using System.Collections.Generic;
using Infrastructure.Core;

namespace DLYB.CA.Contracts.Entity
{
    public class PushHistoryEntity : EntityBase<int>
    {
        public int ContentId { get; set; }

        public string ContentType { get; set; }

        public string UserSelectedType { get; set; }

        public string ToUsers { get; set; }

        public string ToTags { get; set; }

        public string ToDepartments { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string CreatedUserId { get; set; }

        public string UpdatedUserId { get; set; }

        public string TargetType { get; set; }

        public int AppId { get; set; }

        public virtual ICollection<PushHistoryDetailEntity> Details { get; set; }
    }
}
